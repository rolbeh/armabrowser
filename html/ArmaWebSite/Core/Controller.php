<?php
// Core/Controller.php

namespace Core;
/**
 * Controller short summary.
 *
 * Controller description.
 *
 * @version 1.0
 * @author Fake
 */
class Controller
{
    /**
     * Get the Path of the View file, or set it
     * @var String
     */
    public $ViewPath = '';

    public $Layout = 'default';

    /**
     * An array of accept formats
     * @var String[]
     */
    public $AcceptFormats = array();

    private $BaseUrl = '';
    private $Controllername = '';
    private $PageDate;
    private $Title = DEFAULT_TITLE;
    private $PageDescription = DEFAULT_DESCRIPTION;
    

    /**
     * ViewModel data for view renderer
     * @var mixed
     */
    public $ViewModel = null;


    function __construct() {
        $this->AcceptFormats = explode(',', $_SERVER['HTTP_ACCEPT']);
        $this->PageDate = date("c");

        $protocol = '';
        if (isset($_SERVER['HTTPS']) && ($_SERVER['HTTPS'] == 'on' 
            || $_SERVER['HTTPS'] == 1) 
            || isset($_SERVER['HTTP_X_FORWARDED_PROTO']) 
            && $_SERVER['HTTP_X_FORWARDED_PROTO'] == 'https') {
            $protocol = 'https://';
        }
        else {
            $protocol = 'http://';
        }

        $this->BaseUrl = $protocol.$_SERVER['HTTP_HOST'].($_SERVER['SERVER_PORT']!='80'?':'.$_SERVER['SERVER_PORT']:'').rtrim(dirname($_SERVER['PHP_SELF']));
        if ($this->BaseUrl[strlen($this->BaseUrl)-1] != '/')
            $this->BaseUrl .= '/';
    }

    /**
     * Summary of RenderView
     * @return void
     */
    protected function RenderView(){
       
        if($this->Layout && $this->Layout != ''){
            $layoutPath = 'Layout/' . $this->Layout . '.php';

            if (file_exists($layoutPath))
            {
                if (file_exists($this->ViewPath) && include($layoutPath))
                    return;
                echo $this->ViewPath . ' not found! check case-sensitive';
            }
        }else{
            include($this->ViewPath) ;
            return;
        }
        echo $layoutPath . ' not found! check case-sensitive';
    }


    /**
     * return the base URL for installation
     * @return string
     */
    public function GetBaseUrl(){
        return $this->BaseUrl;
    }

    /**
     * return the full URL for the current controller 
     * @return string
     */
    public function GetControllerUrl(){
        return $this->BaseUrl . $this->Controllername .'/';
    }

    /**
     * return the controller name for URL
     * @return string
     */
    public function GetControllerName(){
        return $this->Controllername;
    }

    /**
     * set the controller name for URL
     * @param string $value 
     */
    public function SetControllerName($value){
        $this->Controllername = $value; 
    }

    /**
     * return the title of the current page
     * @return string
     */
    public function GetTitle(){
        return $this->Title;
    }

    /**
     * set the title of the current page
     * @param string $value 
     */
    public function SetTitle($value){
        $this->Title = $value; 
    }

    /**
     * Get the date of the current page as string
     * @return string
     */
    public function GetPageDate(){
        return $this->PageDate;
    }

    /**
     * set the date of the current page as string
     * @param string $value 
     */
    public function SetPageDate($value){
        $this->PageDate = $value; 
    }

    /**
     * Get the description of the current page as string
     * @return string
     */
    public function GetPageDescription(){
        return $this->PageDescription;
    }

    /**
     * Set the description of the current page as string
     * @param string $value 
     */
    public function SetPageDescription($value){
        $this->PageDescription = $value; 
    }
}
