<?php

define('DEFAULT_TITLE', 'Free - Arma 3 Server Browser');
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

/*
if(gethostname() == DEBUGSERVERNAME){
    define('ContentBasePath', 'arma');   
}else
{
    define('ContentBasePath', 'arma');   
}
*/
?>