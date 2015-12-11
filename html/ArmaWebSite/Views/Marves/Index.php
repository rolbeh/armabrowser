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
    throw new Exception("Addons empty "); 
}
?>
<h1 class="title">Marves - Arma 3 Addons repository</h1>
<div style="display: block; padding: 40px; padding-top: 0; vertical-align: top;">
    
    <p>
        Marves is the addon repository for armabrowser and still under construction.<br />
        Do you have questions or suggestion you can send an email. You find the address at the bottom of the page.
    </p>
</div>
<div id="addoncontainer" style="display: block; padding: 40px; padding-top: 0; vertical-align: top;">
 
        <?php include "AddonsTableAjax.php"; ?>

    <script>

        function SortAddons(f) {

            var direction = $(this).data("direction");
            if (!direction ) 
                direction = "asc";

            var url = "<?php echo $this->GetControllerUrl().'AddonsTableAjax';?>";
            var param = {
                url: url,
                element: "#addoncontainer",
                filter: "",
                sorting: f + direction
            };
            LoadAddons(param);

            if (direction == "desc") {
                $(this).data("direction", "asc");
            } else {
                $(this).data("direction", "desc");
            }
        }


        
    </script>
</div>

