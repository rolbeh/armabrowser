<?php

/**
 * Addon Informations
 *
 * @version 1.0
 * @author Fake
 */
class Addon implements JsonSerializable
{
    /**
     * Database Id of Addon 
     * 
     * @var int 
     */
    public $Id = null;

    /**
     * Name of Addon
     * @var string
     */
    public $Name = '';

    /**
     * The hash value of public key
     * @var string hash of PubKey
     */
    public $PubkHash = '';

    /**
     * not set
     * @var string
     */
    public $Version = '';
    
    /**
     * Database id of public key
     * @var int
     */
    public $KeyId= null;

    /**
     * Tag of public key and addon
     * @var string
     */
    public $KeyTag = '';

    /**
     * Datetime of last sigthting on a client
     * @var datetime
     */
    public $Lastsighting = null;

    /**
     * Returns if a file for direct download is present
     * @var boolean
     */
    public $EasyDownload = false;

    /**
     * returns the filesize in bytes or null
     * @var int
     */
    public $Filesize = null;

    /**
     * Returns the count of installations
     * @var int
     */
    public $InstallationCount = 0;


    public function jsonSerialize() {
        return array(
                    'Id' => $this->Id,
                    'Name' => $this->Name,
                    'PubkHash' => $this->PubkHash,
                    'Version' => $this->Version,
                    'KeyId' => $this->KeyId,
                    'KeyTag' => $this->KeyTag,
                    'Lastsighting' => $this->Lastsighting->format(DateTime::ISO8601),
                    'EasyDownload' => $this->EasyDownload,
                    'InstallationCount' => $this->InstallationCount
        );
    }
}
