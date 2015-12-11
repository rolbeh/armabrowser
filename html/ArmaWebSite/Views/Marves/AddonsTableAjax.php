<?php 
require_once 'Classes/Html.php';
require_once 'Controllers/MarvesController.php';

/**
 * @global MarvesController MarvesController 
 * @name $this
 */

/**
 * @global MarvesViewModel MarvesViewModel 
 * @name $viewBag
 */
$viewbag = $this->ViewModel;

if (!(is_array($viewbag->Addons))) { 
    throw new Exception("unexpect class "); 
}

/**
 * <th>Installations</th>
 * <td><?php echo $value->InstallationCount > 0? $value->InstallationCount : ''; ?></td>
 */
?>       <table border="1">
                    <tr>
                        <th onclick="SortAddons('addonname')">Addon name</th>
                        <th >Key name</th>
                        <th onclick="SortAddons('lastsighting')">Last sighting</th>
                        
                        <th></th>
                    </tr>           <?php

                    foreach ($viewbag->Addons as $value)
                    {
                        ?><tr>
                            <td><?php echo $value->EasyDownload? Html::A($this->GetControllerUrl().'details/'.Html::titleToUrlPathSegment($value->Name).'?h='.$value->PubkHash, $value->Name) : $value->Name; ?></td>
                            <td><?php echo $value->KeyTag; ?></td>
                            <td><?php echo $value->Lastsighting->format('d.m.Y'); ?></td>
                            
                            <td><?php echo $value->EasyDownload? Html::A($this->GetControllerUrl().'Download?h='.$value->PubkHash,'Download','title="'.Helper::humanFileSize($value->Filesize,'MB').'"') : ''; ?></td>
                        </tr>
                        <?php
                    }                    
                    
                    ?>
    </table>

