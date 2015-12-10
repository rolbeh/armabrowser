<?php

require_once 'Classes/Exceptions.php';
require_once 'Models/Addon.php';

/**
 * DB short summary.
 *
 * DB description.
 *
 * @version 1.0
 * @author Fake
 */
class DB
{
    /**
     *  the main mysqli DB - connection
     */
    private static $mysql = null;
    
    function InitConnection(){
        
        if(!isset(self::$mysql))
            self::$mysql = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);
        
        /* check connection */
        if (mysqli_connect_errno()) {
            throw new Exception('mysqli connect error');
            exit();
        }        
    }
    
    /**
     * Summary of UpdInstallationsId
     * @param ClientInstallation $clientInstallation 
     * @throws NullExceptions 
     * @return String
     */
    public static function UpdOrInstallationsId($clientInstallation){
        
        if (!isset($clientInstallation)) 
            throw new NullExceptions('$installation_id is null');
        if (!isset($clientInstallation->ClientNr)) 
            throw new NullExceptions('ClientInstallation->ClientNr is null');
        if (!isset($clientInstallation->ClientVersion)) 
            throw new NullExceptions('ClientInstallation->ClientVersion is null');
        if (!isset($clientInstallation->Valid)) 
            throw new NullExceptions('ClientInstallation->Valid is null');
        
        self::InitConnection();
                
        $nr = $clientInstallation->ClientNr;
        $version = $clientInstallation->ClientVersion;
        $id = 0;
        $valid =  $clientInstallation->Valid;
                
        $sqlSelect = 'SELECT client_id FROM  asb_client WHERE client_nr = ?';
        
        $stmt = self::$mysql->prepare($sqlSelect);
        if (!$stmt) throw new Exception(self::$mysql->error);
        $stmt->bind_param('s', $nr);
        
        if ($stmt->execute()){
            if ($stmt->bind_result($id)){
                $res = $stmt->fetch();
                if ($res == true){
                    $clientInstallation->ClientId = $id;
                }
            }
        }
        $stmt->close();
        
        if ($id != 0){
            self::UpdaInstallationsId($clientInstallation);
        }else{
            self::InsertInstallationsId($clientInstallation);
        }
    }
    
    
    /**
     * Summary of UpdInstallationsId
     * @param ClientInstallation $clientInstallation 
     * @throws NullExceptions 
     */
    private static function UpdaInstallationsId($clientInstallation){
        
        if (!isset($clientInstallation)) 
            throw new NullExceptions('$installation_id is null');
        if (!isset($clientInstallation->ClientVersion)) 
            throw new NullExceptions('ClientInstallation->ClientVersion is null');
        if (!isset($clientInstallation->Valid)) 
            throw new NullExceptions('ClientInstallation->Valid is null');
        if (!isset($clientInstallation->ClientId)) 
            throw new NullExceptions('ClientInstallation->ClientId is null');
        
        $version = $clientInstallation->ClientVersion;
        $id = $clientInstallation->ClientId;
        $valid =  $clientInstallation->Valid;
        
        $sqlUpdate = "UPDATE asb_client "
                    ."SET client_lastsighting = NOW() , "
                    ."client_authvalid = ? ,"
                    ."client_version = ? "
                    ."WHERE client_id = ? ";
        
        self::InitConnection();
        
        $stmt = self::$mysql->prepare($sqlUpdate);
        
        $stmt->bind_param('ssd', $valid, $version, $id);
        
        $stmt->execute();
        
        $stmt->close();
    }
    
    /**
     * Append a new recorde in the table asb_client
     * @param string $installation_id 32 HEX as Array of a Guid
     * @throws NullExceptions 
     */
    static function InsertInstallationsId($clientInstallation){
        
        if (!isset($clientInstallation)) 
            throw new NullExceptions('$installation_id is null');
        if (!isset($clientInstallation->ClientNr)) 
            throw new NullExceptions('ClientInstallation->ClientNr is null');
        if (!isset($clientInstallation->ClientVersion)) 
            throw new NullExceptions('ClientInstallation->ClientVersion is null');
        if (!isset($clientInstallation->Valid)) 
            throw new NullExceptions('ClientInstallation->Valid is null');
        
        self::InitConnection();
        
        $sql = 'INSERT INTO asb_client (client_nr, client_created, client_lastsighting, client_authvalid,client_version) '
                .' VALUES (?, NOW(), NOW(),?, ?)';
        
        $version = $clientInstallation->ClientVersion;
        $nr = $clientInstallation->ClientNr;
        $valid =  $clientInstallation->Valid;
        
        $stmt = self::$mysql->prepare($sql);
        
        $stmt->bind_param('sss', $nr, $valid, $version);
        
        $stmt->execute();
        $clientInstallation->ClientId = $stmt->insert_id;        
        $stmt->close();
        
    }
    
    
    /**
     * Summary of InsertUnkownAddons
     * @param ClientAddon[] $clientAddons 
     * @throws NullExceptions 
     */
    public static function InsertUnkownAddons($clientAddons){
        
        if (!isset($clientAddons)) 
            throw new NullExceptions('$clientAddons is null');
        
        $mysqli = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);
        
        $qrySql = "SELECT Count(addon_id) FROM asb_addon\n"
                    . "WHERE addon_name = ? \n"
                    . "AND addon_pubkhash = ?";
        
        $insertSql = "INSERT INTO asb_addon \n" 
                        . "( addon_name, addon_pubkhash) \n" 
                        . "VALUES \n" 
                        . "(?, ?);";
        
        $name = "";
        $hash = "";
        
        $qryStmt = self::$mysql->prepare($qrySql);
        $qryStmt->bind_param("ss", $name, $hash);
        $count = 0;
        $qryStmt -> bind_result($count);
        
        $insertStmt = self::$mysql->prepare($insertSql);
        $insertStmt->bind_param("ss", $name, $hash);
        
        foreach($clientAddons as $item) 
        {
         
            foreach($item->Keys as $addonHash){
                $name = $item->ModName;
                $hash = $addonHash->PubKeyHash;
                $qryStmt->execute();
                $qryStmt->store_result();
                if ($qryStmt->fetch()){
                
                    if ($count == 0){
                        $insertStmt->execute();
                        
                    }
                }
                
                
            }
            
        }
    }
    
    
    /**
     * Update an array of ClientAddon
     * @param array $clientAddons ClientAddon Array  
     * @throws NullExceptions 
     */
    public static function UpdClientAddons($clientAddons){
        
        if (!isset($clientAddons)) 
            throw new NullExceptions('$clientAddons is null');
        
        $ClientId = Auth::$ClientInstallation->ClientId;
        
        self::InitConnection();
        
        // Updateing exsists addons for that client
        {
            
            
            
            $sql = "UPDATE asb_client_addon\n"
                        . "SET client_addon_lastsighting = NOW(),\n"
                        . "client_addon_name = ? ,\n"
                        . "client_addon_displaytext = ? ,\n"
                        . "client_addon_version = ? \n"
                . "WHERE client_id = ? "
                . "  AND client_addon_displayname = ?";
            

            $stmt = self::$mysql->prepare($sql);

            if (!$stmt) throw new Exception(self::$mysql->error);
           
            $Name = ''; 
            $DisplayName = ''; 
            $DisplayText = '';  
            $Version = '';  
            $Id = 0;  
            
            $stmt->bind_param('sssss', $Name, $DisplayText, $Version, $ClientId, $DisplayName);

            $insertItems = array();
            
            foreach($clientAddons as $item) 
            {
                    $Name = $item->Name;
                    $DisplayName = $item->ModName;
                    $DisplayText = $item->DisplayText;
                    $Version = $item->Version;
                    
                    $sqlRes=  $stmt->execute();
                    
                    if (!isset($stmt->affected_rows) || $stmt->affected_rows == 0) {
                            array_push($insertItems, $item);
                        }
                }
            $stmt->close();
        }
        
        // Insert new addons
        if (sizeof($insertItems) > 0)
        {
            $sql = 'INSERT INTO asb_client_addon '
                        . '(client_id, '
                        . ' client_addon_name, '
                        . ' client_addon_displayname, '
                        . ' client_addon_displaytext, '
                        . ' client_addon_version, '
                        . ' client_addon_lastsighting) '
                    . 'VALUE '
                    . '('
                    . '	?,'
                    . '	?,'
                    . '	?,'
                    . '	?,'
                    . '	?,'
                    . '	NOW() '
                    .')';
            
            $stmt = self::$mysql->prepare($sql);
             
            $Name = ''; 
            $DisplayName = ''; 
            $DisplayText = '';  
            $Version = '';  
            
            $stmt->bind_param('dssss',
                                $ClientId,
                                $Name,
                                $DisplayName, 
                                $DisplayText, 
                                $Version
                                );
            foreach($insertItems as $item) 
            {
                    $Name = $item->Name;
                    $DisplayName = $item->ModName;
                    $DisplayText = $item->DisplayText;
                    $Version = $item->Version;
                    
                    $sqlRes=  $stmt->execute();
                    
                    $item->Id =  $stmt->insert_id;
            }
            
            $res = $stmt->close();
            
            
            
            $sql = 'INSERT INTO asb_client_addon_keys '
                   .'(client_addon_keys_id, client_addon_id, addon_keys_id) '
                   .' VALUES (NULL, ?, ?) ';
            $stmt = self::$mysql->prepare($sql);
            $key_id = 0;
            $claddonid = 0;
            $stmt->bind_param('dd',
                               $claddonid,
                               $key_id);
            
            foreach($insertItems as $item) 
            {
                foreach($item->Keys as $KeyItem) 
                {
                    $key_id = $KeyItem->Id;
                    $claddonid = $item->Id;
                    $sqlRes=  $stmt->execute();
                }
            }
            
        }
    }
    
    
    /**
     *  GetOrInsert
     * @param AddonKey $addonKeys one AddonKey  
     * @throws NullExceptions  $addonKeys is null
     * @throws Exception , some is wrong with SQL 
     */
    public static function GetOrInsertAddonKey($addonKeys){
        
        if (!isset($addonKeys)) 
            throw new NullExceptions('$clientAddons is null');
        if (!isset($addonKeys->PubKeyHash)) 
            throw new NullExceptions('AddonKey::PubKeyHash is null');
        
        $mysql = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);
        
        $pubKeyHash = $addonKeys->PubKeyHash;  
         
         $sql = "SELECT \n"
                        . "\n"
                        . "addon_keys_id\n"
                        . "\n"
                        . "FROM asb_addon_keys WHERE addon_key_pubkhash = ?";
        

         $stmt = $mysql->prepare($sql);
        
         if (!$stmt) throw new Exception($mysql->error);
        
 
        $stmt->bind_param('s',
                            $pubKeyHash
                            );
         
        if ($stmt->execute()){
            if ($stmt->bind_result($addon_keys_id)){
                $res = $stmt->fetch();
                if ($res == true){
                    $addonKeys->Id = $addon_keys_id;
                }
            }
        }
        $stmt->close();    
            
        if(!isset($addonKeys->Id)){
            $sql = "INSERT INTO asb_addon_keys\n"
                    . "	(addon_key_tag, \n"
                    . " addon_key_pubkhash, \n"
                    . " addon_key_pubkh) \n"
                    . "	VALUES (?,?,?)";
            $stmt->close();
            $stmt = $mysql->prepare($sql);
            if (!$stmt) throw new Exception($mysql->error);
        
            $tag = $addonKeys->Tag;
            $pubKey = $addonKeys->PubKey;
        
            $stmt->bind_param('sss',
                                $tag,
                                $pubKeyHash,
                                $pubKey
                                );
        
            $res = $stmt->execute();
            if($res == true && $stmt->affected_rows == 1){
                $addonKeys->Id = $stmt->insert_id;
            }
            $stmt->close();
         }
    }
    
    /**
     * Summary of GetUsedAddons
     * @return Addon[]
     */
    public static function GetUsedAddons($sorting = ''){
        
        $sql = "SELECT \n"
            . "	asb_addon.addon_id\n"
            . "	, asb_addon.addon_name\n"
            . "	, asb_addon.addon_pubkhash\n"
            . "	, asb_addon.addon_version\n"
            . "	, asb_addon_keys.addon_keys_id\n"
            . "	, asb_addon_keys.addon_key_tag\n"
            . "	, MAX(asb_client_addon.client_addon_lastsighting) as client_addon_lastsighting\n"
            . " , COUNT(asb_client_addon.client_id) as installation_count\n"
            . "FROM asb_addon\n"
            . " INNER JOIN asb_addon_keys on asb_addon.addon_pubkhash = asb_addon_keys.addon_key_pubkhash\n"
            . " INNER JOIN asb_client_addon_keys on asb_addon_keys.addon_keys_id = asb_client_addon_keys.addon_keys_id\n"
            . " INNER JOIN asb_client_addon on asb_client_addon_keys.client_addon_id = asb_client_addon.client_addon_id\n"
            . " group by asb_addon.addon_id\n"
            . " , asb_addon.addon_name\n"
            . " , asb_addon.addon_pubkhash\n"
            . " , asb_addon.addon_version\n"
            . " , asb_addon_keys.addon_keys_id\n"
            . " , asb_addon_keys.addon_key_tag\n";

        if($sorting == 'addonnameasc')
            $sql .= " ORDER BY asb_addon.addon_name \n";
        elseif($sorting == 'addonnamedesc')
            $sql .= " ORDER BY asb_addon.addon_name DESC \n";
        elseif($sorting == 'lastsightingasc')
            $sql .= " ORDER BY asb_client_addon.client_addon_lastsighting \n";
        elseif($sorting == 'lastsightingdesc')
            $sql .= " ORDER BY asb_client_addon.client_addon_lastsighting DESC \n";

         $sql .= "\n"
            . "";
        
        $rows = array();
        
        $mysqli = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);
        try
        {
            $result = $mysqli->query($sql);
            if ($result)
            {
                while($row = $result->fetch_array(MYSQLI_ASSOC))
                {
                    $item = new Addon();
                    $item->Id = intval($row['addon_id']);
                    $item->Name = $row['addon_name'];
                    $item->PubkHash = $row['addon_pubkhash'];
                    $item->Version = $row['addon_version'];
                    $item->KeyId = intval($row['addon_keys_id']);
                    $item->KeyTag = $row['addon_key_tag'];
                    $item->Lastsighting = date_create_from_format('Y-m-d H:i:s', $row['client_addon_lastsighting']);
                    $item->InstallationCount = intval($row['installation_count']);
                    $rows[] = $item;
                }
            }
        }
        catch (Exception $exception)
        {
            header("HTTP/1.0 500 Server Error");
        }
        
        if  ($mysqli) $mysqli->close();
        
        return $rows;
    }
    
    /**
     * Summary of GetUsedAddonInfos
     * @param array $tagNames KeyTag or hash of public key
     * @return array
     */
    public static function GetUsedAddonInfos($tagNames){
        
        $sql = "SELECT addon_key_tag\n"
            . "	, asb_addon.addon_name\n"
            . "	, asb_addon.addon_version\n"
            . "	, asb_addon_uris.addon_uri_ref\n"
            . " , asb_addon.addon_pubkhash \n"
            . "FROM asb_addon_keys \n"
            . "	INNER JOIN asb_addon ON asb_addon_keys.addon_key_pubkhash = asb_addon.addon_pubkhash\n"
            . "	LEFT JOIN asb_addon_uris ON asb_addon.addon_id = asb_addon_uris.addon_id\n"
            . "WHERE asb_addon_keys.addon_key_tag = ? \n"
            . " OR asb_addon.addon_pubkhash = ? ";

        
        $rows = array();
        
        $mysqli = new mysqli(DB_HOST, DB_USER, DB_PASSWORD, DB_NAME);
        try
        {
            $stmt = $mysqli->stmt_init();
            if($stmt->prepare($sql))
            {
                //http://www.peterkropff.de/site/php/mysqli_stmt_daten.htm
                if ($stmt->bind_param('ss', $tagName, $tagName))
                {
                    foreach ($tagNames as $tagName)
                    {
                        if ($stmt->execute())
                        {                
                            
                            $stmt -> bind_result($addon_key_tag, $addon_name, $addon_version, $addon_uri_ref, $addon_pubkhash);
                            while ($stmt->fetch()) {
                                $row = array();
                                $row["keytag"] = $addon_key_tag;
                                $row["name"] = $addon_name;
                                $row["version"] = $addon_version;
                                $row["uriref"] = $addon_uri_ref;
                                $row["hash"] = $addon_pubkhash;
                                $rows[] = $row;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception $exception)
        {
            header("HTTP/1.0 500 Server Error");
        }
        
        if  ($mysqli) $mysqli->close();
        
        return $rows;
    }
}
