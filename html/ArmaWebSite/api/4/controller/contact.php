<?php

/**
 * about short summary.
 *
 * about description.
 *
 * @version 1.0
 * @author Fake
 */
class Contact
{
    public function __construct(){
        echo 'This is the about page </br>';
        $this->_other();
    }   
    
    
    protected function _other(){
        echo 'this is the other function in class about </br>';
    }
}
