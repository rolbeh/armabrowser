<?php

// ** MySQL settings ** //

if(gethostname() == 'debugserver'){
    define('DB_NAME', '');     // The name of the database
    define('DB_USER', '');     // Your MySQL username
    define('DB_PASSWORD', ''); // ...and password
    define('DB_HOST', '');     // 99% chance you won't need to change this value
    define('DB_CHARSET', '');
    define('DB_COLLATE', '');
}else{
    define('DB_NAME', '');     // The name of the database
    define('DB_USER', '');     // Your MySQL username
    define('DB_PASSWORD', ''); // ...and password
    define('DB_HOST', '');     // 99% chance you won't need to change this value
    define('DB_CHARSET', 'utf8');
    define('DB_COLLATE', '');
}

define('ADDON_FilePath', 'addonfiles/');

define('AUTH_PRIVATE', 
    '');



?>