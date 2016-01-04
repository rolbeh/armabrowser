<?php
require_once 'route.php';
require_once 'config.php';
require_once 'Classes/Exceptions.php';
require_once 'Classes/Model/AddonKey.php';
require_once 'Classes/Model/ClientInstallation.php';
require_once 'Classes/Model/ClientAddon.php';



Route::AddRoute('/Addons', 'Addons');

/**
 *
 *
 *
 *
 * @version 2.0
 * @author Fake
 */
class Addons
{
    public function __construct(){


    }

    public function GET(){

        if (substr_count($_SERVER['HTTP_ACCEPT'], 'application/json') == 1
            or strcasecmp($_SERVER[HTTP_ACCEPT], '*/*') == 0 )
        {
            
            $result = json_encode(DB::GetUsedAddons());
            print $result;
        }
    }

    public function POST(){

        if (substr_count($_SERVER['HTTP_ACCEPT'], 'application/json') == 1
            or strcasecmp($_SERVER[HTTP_ACCEPT], '*/*') == 0 )
        {
            $body = '';
            $f = @fopen("php://input", "r");
            $body = stream_get_contents($f);
            fclose($f);

            $array = json_decode($body);
            $clientAddons = array();
            
            
            
            foreach($array as $key=>$addon) {

                foreach($addon->Keys as $key=>$addonKey) {
                    $addonKeyItem = new AddonKey();
                    $addonKeyItem->Tag = $addonKey->Key;
                    $addonKeyItem->PubKeyHash = sha1($addonKey->PubK);
                    $addonKeyItem->PubKey = $addonKey->PubK;

                    DB::GetOrInsertAddonKey($addonKeyItem);

                    //$addonKey->PubKeyHash = sha1($addonKey->PubK);
                    $addonKey->PubKeyHash = $addonKeyItem->PubKeyHash;

                    $addon->Keys[$key] = $addonKeyItem;
                }

            }


            foreach($array as $key=>$addon) {

                $item = new ClientAddon();
                $item->ClientNr = Auth::$ClientInstallation->ClientNr ;
                $item->Name = $addon->Name;
                $item->ModName = $addon->ModName;
                $item->DisplayText = $addon->DisplayText;
                $item->Version = $addon->Version;
                $item->Keys = $addon->Keys;

                //PubKeyHash hinzufügen
                // $item->$Keys

                $clientAddons[$key] = $item;
            }
            DB::InsertUnkownAddons($clientAddons);
            DB::UpdClientAddons($clientAddons);



        }else
            header("HTTP/1.0 415 Unsupported Media Type");
    }
    
    public function AddonInfo(){
        if (substr_count($_SERVER['HTTP_ACCEPT'], 'application/json') == 1
                or strcasecmp($_SERVER[HTTP_ACCEPT], '*/*') == 0 )
        {
            $body = '';
            $f = @fopen("php://input", "r");
            $body = stream_get_contents($f);
            fclose($f);

            $array = json_decode($body);
             
            $result = DB::GetUsedAddonInfos($array);


            foreach ($result as &$row)
            {
                $row["easyinstall"] = file_exists(ADDON_FilePath.$row["hash"].'.zip');
            }
            
                
            print(json_encode($result));
            
        }
    }


    public function UploadAddon(){
        if (!(substr_count($_SERVER['HTTP_ACCEPT'], 'application/json') == 1
             or strcasecmp($_SERVER['HTTP_ACCEPT'], '*/*') == 0 )) 
        {
            header("HTTP/1.0 400");
            return;
        }
        header('Content-Type: text/plain');
        try
        {
            $hash = isset($_POST['hash'])? $_POST['hash'] : null;
            if (!isset($hash)){
                print('{"Error" : "NOHASH"}');
                header("HTTP/1.0 400");
                return;
            }

            $myfile = isset($_FILES[$hash]) ? $_FILES[$hash] : null;
            if (!isset($myfile)){
                print('{"Error" : "NOFILE"}');
                header("HTTP/1.0 400");
                return;
            }

            if ($myfile['size'] > 104857600){
                print('{"Error" : "FILETOBIG"}');
                header("HTTP/1.0 400 file to big");
                return;
            }

            print('{"status" : "OK angekommen ' . $hash .'"}');
            move_uploaded_file($myfile['tmp_name'],ADDON_FilePath.$hash.'.zip');

            //print("\n\rcopied " . ADDON_FilePath.$hash.'.zip');

         }
        catch (Exception $exception)
        {
            header("HTTP/1.0 500 Server Error " . $exception);
        }
        
    }

    public function BigUploadAddon(){
        if (!(substr_count($_SERVER['HTTP_ACCEPT'], 'application/json') == 1
             or strcasecmp($_SERVER['HTTP_ACCEPT'], '*/*') == 0 )) 
        {
            header("HTTP/1.0 400");
            return;
        }
        header('Content-Type: text/plain');
        try
        {
            $hash = isset($_POST['hash'])? $_POST['hash'] : null;
            if (!isset($hash)){
                print('{"Error" : "NOHASH"}');
                header("HTTP/1.0 400");
                return;
            }

            $myfile = isset($_FILES[$hash]) ? $_FILES[$hash] : null;
            if (!isset($myfile)){
                print('{"Error" : "NOFILE"}');
                header("HTTP/1.0 400");
                return;
            }

            if ($myfile['size'] > 104857600){
                print('{"Error" : "FILETOBIG"}');
                header("HTTP/1.0 400 file to big");
                return;
            }

            print('{"status" : "OK angekommen ' . $hash .'"}');
            move_uploaded_file($myfile['tmp_name'], ADDON_FilePath.$hash.'.zip');

            //print("\n\rcopied " . ADDON_FilePath.$hash.'.zip');

        }
        catch (Exception $exception)
        {
            header("HTTP/1.0 500 Server Error " . $exception);
        }
        
    }

    public function DownloadAddon(){
        try
        {
            if ($_SERVER['HTTP_ACCEPT'] != 'application/zip')
            {
                header("HTTP/1.0 400");
                return;
            }

            $hash = $_POST['hash'];
            $filePath = 'addonfiles/' . $hash . '.zip';

            if (!file_exists($filePath)){
                header("HTTP/1.0 404");
                return;
            }

            $array = array();
            $array[] = $hash;

            $result = DB::GetUsedAddonInfos($array);
            if (count($result) == 0){
                header("HTTP/1.0 404");
                return;
            }

            $item = $result[0];

            header("Accept-Ranges:  bytes");
            header("Pragma: ");
            header("Cache-Control: ");
            header("Content-Length: " . filesize($filePath));
            header("Content-MD5: " . md5_file($filePath));
            header("Content-Type: application/zip");
            header("Content-Disposition: attachment; filename=\"".$item["name"].".zip\"");
            set_time_limit(0);
            $file = @fopen($filePath,"rb");
            while(!feof($file))
            {
                print (@fread($file, 1024*8));
                //ob_flush();
                flush();
            }
            @fclose($file);
            flush();
        }
        catch (Exception $exception)
        {
            header("HTTP/1.0 500 Server Error " . $exception);
        }

    }
}
