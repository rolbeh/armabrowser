<?php

header('Content-Type: application/json');

if (substr_count($_SERVER['HTTP_ACCEPT_ENCODING'], 'gzip')){
		ob_start("ob_gzhandler");
	}else{
		 ob_start();
	}


require_once  'config.php';
require_once  'Auth.php';




if (Auth::$VALID == false)
{   
    //Unauthorized = 401
    header("HTTP/1.0 401 Unauthorized");
    exit();
}

require_once  'route.php';

//require_once  'controller/home.php';
require_once  'controller/Addons.php';
//require_once  'controller/contact.php';


$route = Route::GetRoute();


$route->submit();

?>