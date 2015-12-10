<?php

$GLOBALS['StartMicrotime'] = microtime(true);

//if (substr_count($_SERVER['HTTP_ACCEPT_ENCODING'], 'gzip')){
//    ob_start("ob_gzhandler");
//}else{
//    ob_start();
//}

require_once  'core.php';

include_once  'Classes/compatibility.php';
require_once  'Classes/DB.php';

require_once  'Controllers/HomeController.php';
require_once  'Controllers/MarvesController.php';

$route = Route::GetRoute();

$route->submit();


?>