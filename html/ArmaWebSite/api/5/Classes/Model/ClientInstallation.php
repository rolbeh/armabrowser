<?php

/**
 * ClientInstallation short summary.
 *
 * ClientInstallation description.
 *
 * @version 1.0
 * @author Fake
 */
class ClientInstallation
{
    public $ClientId = ''; 
    
    /**
     * Eindeutige Clientidentification
     */
    public $ClientNr = ''; 
    
    /**
     * Check the HTTP_APPKEY for valid information
     *  
     */
    public $ClientVersion = '';
    
    public $Valid = false;
}
