<?php 
if (strcasecmp(basename($_SERVER['PHP_SELF']), 'index.php') != 0) { header("HTTP/1.0 404 Not Found"); exit();}

require_once 'Classes/Html.php';
require_once 'Controllers/MarvesController.php';

/**
 * @global MarvesController MarvesController 
 * @name $this
 */

/**
 * @global array 
 * @name $viewBag
 */
$viewbag = $this->ViewModel;

if (!(is_array($viewbag))) { 
    throw new Exception("unexpect class "); 
}
?>
<h1 class="title">File: <?php echo $viewbag['item']['name']; ?> for Arma 3</h1>
<div style="min-width: 200px; margin-left: 5px; margin-right: auto; margin-top: 10px; margin-bottom: 20px;">
    
    <span style="font-size: 20pt; display:block; text-align:center"><?php  echo Html::A($this->GetControllerUrl().'Download?h='.$viewbag['item']['hash'],'Download'); ?></span>
    <br />
    Size: <?php  echo Helper::humanFileSize($viewbag['filesize'],'MB'); ?>
    <br />
    <br />
    <?php 
    if (isset($viewbag['files'])) {
        foreach ($viewbag['files'] as $key =>  $value)
        {
            echo '<header>'.$key.'</header>';
            echo '<acticle><pre>'.$value.'</pre></article>';
        }
    }
    ?>
</div>