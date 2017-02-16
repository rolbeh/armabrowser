<?php

include_once('Models/MarvesViewModel.php');

Core\Route::AddRoute('/Marves', 'Marves');

/**
 * Marves is the addon manager
 *
 * Marves is the addon manager
 *
 * @version 1.0
 * @author Fake
 */
class MarvesController extends Core\Controller
{

    /**
     * The Api-Version of Armabrowser, needs for get the location of addon files
     * @var string
     */
    private $ApiVersion = '4';

    
    function __construct() {
        parent::__construct();
         
    }

    public function Index(){
        
        $this->SetTitle(DEFAULT_TITLE . "Marves - Arma 3 Addons repository");
        $dbResult = DB::GetUsedAddons();

        foreach ($dbResult as $value)
        {
            $value->EasyDownload = file_exists('api/'.$this->ApiVersion.'/addonfiles/'.$value->PubkHash.'.zip');
            if ($value->EasyDownload)
                $value->Filesize = filesize('api/'.$this->ApiVersion.'/addonfiles/'.$value->PubkHash.'.zip');
        }
        
        $result = $dbResult;

        $this->ViewModel = new MarvesViewModel();
        $this->ViewModel->Addons = $result;

        if (isset($_GET['$format'])){
            $this->AcceptFormats = array('application/'.$_GET['$format']);
        }

        foreach ($this->AcceptFormats as $acceptFormat)
        {

            if (substr_compare('text/html', $acceptFormat, 0, 9, true ) == 0 ){
                // Wenn kein spezielles Format gewünschte wurde.
                header('Content-Type: text/html');
                //uasort($this->ViewModel->Addons, function (&$a, &$b) { if ($a->Lastsighting == $b->Lastsighting) { return 0; } return ($a->Lastsighting > $b->Lastsighting) ? -1 : 1;} );
                $this->RenderView();
                return;
            }

            if (substr_compare('application/json', $acceptFormat, 0, 16, true ) == 0 ){
                
                header('Content-Type: application/json');
                header('Cache-Control: private, no-cache, no-store, must-revalidate');
                
                if (isset($_SERVER['HTTP_REFERER'])){
                    if (substr_compare($_SERVER['HTTP_REFERER'],"http://localhost:59676", 0, 22, true) == 0){
                        header('Access-Control-Allow-Origin: http://localhost:59676');
                    }

                    if (substr_compare($_SERVER['HTTP_REFERER'],"http://armabrowser.org/Marves", 0, 29, true) == 0){
                        header('Access-Control-Allow-Origin: http://armabrowser.org/Marves');
                    }
                }
                

                 
                echo '[';
                $resultstr = array();
                foreach ($this->ViewModel->Addons as $value) {
                    if ($value instanceof JsonSerializable){
                        $resultstr[] = json_encode($value->jsonSerialize());
                        continue;
                    }
                    $resultstr[] = json_encode($value);
                }
                $result = implode(",",$resultstr);
                echo $result;

                print ']';
                

                
                return;
            }

            if (substr_compare('application/xml', $acceptFormat, 0, 15, true ) == 0 ){
                
                header('Content-Type: application/xml');
                $xml = new SimpleXMLElement('<addons/>');       
                foreach ($this->ViewModel->Addons as $value)
                {
                    $addonElement = $xml->addChild('addon');
                    $addonElement->addChild('Id', $value->Id);
                    $addonElement->addChild('InstallationCount', $value->InstallationCount);
                    $addonElement->addChild('KeyId', $value->KeyId);
                    $addonElement->KeyTag = $value->KeyTag;
                    $addonElement->addChild('Lastsighting', $value->Lastsighting->format('d.m.Y'));
                    //$addonElement->addChild('Name', $value->Name);
                    $addonElement->Name = $value->Name;
                    $addonElement->addChild('PubkHash', $value->PubkHash);
                    $addonElement->Version = $value->Version;
                }
                

                print $xml->asXML();
                return;
            }
        }

        // Wenn kein spezielles Format gewünschte wurde.
        header('Content-Type: text/html');
        uasort($this->ViewModel->Addons, function (&$a, &$b) { if ($a->Lastsighting == $b->Lastsighting) { return 0; } return ($a->Lastsighting > $b->Lastsighting) ? -1 : 1;} );
        $this->RenderView();
    }


    public function Download(){
        try
        {
            //if ($_SERVER['HTTP_ACCEPT'] != 'application/zip')
            //{
            //    header("HTTP/1.0 400");
            //    return;
            //}


            if (!isset($_GET['h'])){
                $host  = $_SERVER['HTTP_HOST'];
                $uri   = rtrim(dirname($_SERVER['PHP_SELF']), '/\\');
                header("Location: http://$host$uri");
                exit;
            }

            $hash = $_GET['h'];

            $filePath = $this->getAddonFilePath($hash);
            if (!$filePath){
                header("HTTP/1.0 404");
                return;
            }

            $item = $this->getAddonItem($hash);
            if (!$item){
                header("HTTP/1.0 404");
                return;
            }

            //header("Location: ". $this->GetBaseUrl()."/".$filePath);

            header("Accept-Ranges:  bytes");
            header("Content-Length: " . filesize($filePath));
            header("Content-Type: application/zip");
            header("Content-Disposition: attachment; filename=\"".$item["name"].".zip\";");
            
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

    /**
     * return the file path of the addon-file
     * @param string $hash 
     * @return bool|string
     */
    function getAddonFilePath($hash){

        $filePath = 'api/'.$this->ApiVersion.'/addonfiles/'.$hash.'.zip';
        if (!file_exists($filePath)){
            return false;
        }
        return $filePath;
    }

    /**
     * Return the Addon entry from Database
     * @param string $hash 
     * @return array|bool Array with Information about an addon, or false
     */
    private function getAddonItem($hash){
        
        $array = array();
        $array[] = $hash;

        $result = DB::GetUsedAddonInfos($array);
        if (count($result) == 0){
            return false;
        }
        return $result[0];
    }

    public function details(){

        if (!isset($_GET['h'])){
            $host  = $_SERVER['HTTP_HOST'];
            $uri   = rtrim(dirname($_SERVER['PHP_SELF']), '/\\');
            header("Location: http://$host$uri");
            exit;
        }

        $hash = $_GET['h'];

        $filePath = $this->getAddonFilePath($hash);
        if (!$filePath){
            header("HTTP/1.0 404");
            return;
        }

        $item = $this->getAddonItem($hash);
        if (!$item){
            header("HTTP/1.0 404");
            return;
        }

        $viewBag['hash'] = $hash;
        $viewBag['item'] = $item;

        $filePath = $this->getAddonFilePath($hash);
        if (!$filePath){
            header("HTTP/1.0 404");
            return;
        }
        $viewBag['filePath'] = $filePath;
        $viewBag['filesize'] = filesize($filePath);

        $zip = new ZipArchive; 
        if ($zip->open($filePath ))//'api/'.$this->ApiVersion.'/addonfiles/'.$hash.'.zip')) 
        { 
            $re =             "/^[^\\\\]+(\\\\mod\\.cpp)$/i"; // find mod.cpp in first subdir -- (@CBA_A3_v2.0.0.150817\mod.cpp), ignore all other sub-subdirs
            $regPatternText = "/^[^\\\\\\/]+[\\\\\\/]([^\\\\\\/]+\\.(txt|cpp))$/im"; // find *.txt in first subdir -- (@CBA_A3_v2.0.0.150817\readme.txt), ignore all other sub-subdirs
            for($i = 0; $i < $zip->numFiles; $i++) 
            {                   
                $innerFilePath = $zip->getNameIndex($i);
                $matches = array();
                if (preg_match($regPatternText, $innerFilePath, $matches)){
                    
                    $fp =  $zip->getStream($innerFilePath);
                    if (!$fp) exit("HTTP/1.0 404");

                    $fileContent = '';
                    while (!feof($fp)) {
                        $fileContent .= fread($fp, 100);
                    }

                    fclose($fp);

                    $viewBag['files'][$matches[1]] = $fileContent;
                }
            }
        }
        
        $this->SetTitle(DEFAULT_TITLE . "Marves - Details of " . $viewBag['item']['name']);
        $this->ViewModel = $viewBag;
        header('Content-Type: text/html');
        $this->RenderView();

    }


    public function AddonsTableAjax(){

        $sorting = '';
        if(isset($_POST['sorting'])) {
            $sorting = $_POST['sorting'];
        }

        $dbResult = DB::GetUsedAddons($sorting);

        foreach ($dbResult as $value)
        {
            $value->EasyDownload = file_exists('api/'.$this->ApiVersion.'/addonfiles/'.$value->PubkHash.'.zip');
            if ($value->EasyDownload)
                $value->Filesize = filesize('api/'.$this->ApiVersion.'/addonfiles/'.$value->PubkHash.'.zip');
        }
        
        $result = $dbResult;

        $this->ViewModel = new MarvesViewModel();
        $this->ViewModel->Addons = $result;

        if (isset($_GET['$format'])){
            $this->AcceptFormats = array('application/'.$_GET['$format']);
        }

        $this->Layout = '';
        $this->RenderView();
    }
}
?>