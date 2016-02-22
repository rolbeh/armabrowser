<?php

require_once 'Classes/DB.php';
require_once 'Classes/Model/ClientInstallation.php';


define('AUTH_InstallationToken','HTTP_APPI'); 
define('AUTH_InstallationVersion','HTTP_CLIENTVER'); 
define('AUTH_APPKEY','HTTP_APPKEY');

/**
 * Auth short summary.
 *
 * Auth description.
 *
 * @version 1.0
 * @author Fake
 */
class Auth{
    
    public static $VALID = false;
    
    public static $ClientInstallation = '';
    
    public static function CheckAppKey()
    {
    
        $clientInstallation = new ClientInstallation();
        
        if (isset($_SERVER['HTTP_POSTMAN_TOKEN']) AND $_SERVER['HTTP_POSTMAN_TOKEN'] == 'f0e69c11-f794-0487-201f-4891dd24f16e')
        {
            $clientInstallation->ClientNr = '0016a701abc3d94a8fb6f020974b8ffa';
            $clientInstallation->ClientVersion = '0.6.8.76';
            self::$VALID = true;
            $clientInstallation->Valid = self::$VALID;
            DB::UpdOrInstallationsId($clientInstallation);
            self::$ClientInstallation = $clientInstallation;
            return self::$VALID;
        }
        
        if (!isset($_SERVER[AUTH_InstallationToken]) || strlen($_SERVER[AUTH_InstallationToken]) != 32 )
        {
            return self::$VALID;  
        }        
        
        $clientInstallation->ClientNr = $_SERVER[AUTH_InstallationToken];
        $clientInstallation->ClientVersion = $_SERVER[AUTH_InstallationVersion];
        
        if (isset($_SERVER[AUTH_APPKEY]))
        {
            $appkey = base64_decode($_SERVER[AUTH_APPKEY]);
            $res = openssl_get_privatekey(AUTH_PRIVATE);
            openssl_private_decrypt($appkey,$newsource,$res);
    
            $myTime = time();
            
            $appvalid = false;
            for ($i = -3; $i < 4; $i++)
            {
                $token = 'ArmaServerBrowser "'.gmdate('D, d M Y H:i:00 \G\M\T', $myTime - ($i*60)). '" 2 ' .$_SERVER[AUTH_InstallationToken] ;
                if ($token == $newsource)
                {
                    self::$VALID = true;
                    break;
                }
                //'ArmaServerBrowser Mon, 23 Feb 2015 22:32:00 GMT 1''
            }
        }    
        
        $clientInstallation->Valid = self::$VALID;
        self::$ClientInstallation = $clientInstallation;
        DB::UpdOrInstallationsId($clientInstallation);
        return self::$VALID;
    }
}

Auth::CheckAppKey();

