<?php


if( version_compare(PHP_VERSION, '5.4.0', '<')){
    interface JsonSerializable {
        
        public function jsonSerialize();
        
    }
}
?>