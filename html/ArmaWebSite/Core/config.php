<?php

define('DEFAULT_TITLE', 'Arma 3 Server Browser - ');
define('DEFAULT_DESCRIPTION', 'Find and join arma 3 server as fast as you can! Easy administration of all your Addons and mods.');

// ** MySQL settings ** //

define('DEBUGSERVERNAME', 'homeserver');


if(gethostname() == DEBUGSERVERNAME){
    define('DB_NAME', 'arma');    // The name of the database
    define('DB_USER', 'arma');     // Your MySQL username
    define('DB_PASSWORD', 'pass'); // ...and password
    define('DB_HOST', 'localhost');    // 99% chance you won't need to change this value
    define('DB_CHARSET', 'utf8');
    define('DB_COLLATE', '');
}else{
    define('DB_NAME', 'usr_web239_2');    // The name of the database
    define('DB_USER', 'web239');     // Your MySQL username
    define('DB_PASSWORD', 'Kjd53-J4&Hs'); // ...and password
    define('DB_HOST', 'localhost');    // 99% chance you won't need to change this value
    define('DB_CHARSET', 'utf8');
    define('DB_COLLATE', '');
}

define('AUTH_PRIVATE', 
    '-----BEGIN PRIVATE KEY-----
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
AHnEjv8oAnT2lLIVS8XEWz9/VDoZ0g==
-----END PRIVATE KEY-----');