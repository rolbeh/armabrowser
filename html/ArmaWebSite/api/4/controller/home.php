<?php

/**
 * about short summary.
 *
 * about description.
 *
 * @version 1.0
 * @author Fake
 */
class Home
{
    public function __construct(){
        echo 'This is the Home page </br>';
        $this->_other();
    }   
    
    
    protected function _other(){
        echo 'this is the other function in class Home </br>';
    }
}
