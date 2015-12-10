<?php
include "CheckEntryScript.php";

/**
 * Html short summary.
 *
 * Html description.
 *
 * @version 1.0
 * @author Fake
 */
class Html
{
    /**
     * <a href="'.$link.'" '.$appendAttrs.'>'.$text.'</a>
     * @param string $link 
     * @param string $text 
     * @return string
     */
    static function A($link, $text, $appendAttrs = ''){
        return '<a href="'.$link.'" '.$appendAttrs.'>'.$text.'</a>';
    }
}

class Helper{

    /**
     * returns a human readable filesize as string
     * @param int $size 
     * @param string $unit KB | MB | GB
     * @return string
     */
    static function humanFileSize($size,$unit="") {
        if( (!$unit && $size >= 1<<30) || $unit == "GB")
            return number_format($size/(1<<30),2)."GB";
        if( (!$unit && $size >= 1<<20) || $unit == "MB")
            return number_format($size/(1<<20),2)."MB";
        if( (!$unit && $size >= 1<<10) || $unit == "KB")
            return number_format($size/(1<<10),2)."KB";
        return number_format($size)." bytes";
    }
}