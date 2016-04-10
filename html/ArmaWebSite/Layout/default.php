<?php 
require_once 'Classes/Html.php';
include_once('Core/Exceptions.php');
include_once('Core/Controller.php');

/**
 * @global Core\Controller Controller-Base class
 * @name $this
 */
 
if (!($this instanceof Core\Controller)) { throw new UnexpectedValueException("Invalid operation, expect class Controller"); }

?>
<!DOCTYPE html>
<html style="height: 100%;">
<head>
    <title><?php echo $this->GetTitle(); ?></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="keywords" lang="en" content="arma, arma 3, arma3, server, browser, launcher, addons, start, launch, mods" />
    <meta http-equiv="expires" content="43200" />
    <meta name="date" content="<?php  echo $this->GetPageDate() ?>" />
    <meta name="description" content="<?php  echo $this->GetPageDescription() ?>" />
    
    <link rel="stylesheet" type="text/css" href="<?php echo $this->GetBaseUrl(); ?>Content/css/styles.css">

    <script src="<?php echo $this->GetBaseUrl(); ?>js/jquery-2.1.4.min.js"></script>
    <script src="<?php echo $this->GetBaseUrl(); ?>js/ajax.js"></script>
     
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g;
            m.parentNode.insertBefore(a, m);
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-57695932-1', 'auto');
        ga('send', 'pageview');

    </script>

</head>
<body style="margin: 0; background-color:#363636; color:#C2C2C2; height: 100%; width: 100%;">
    <div style="min-height: 100%; margin-bottom: -100px; padding-bottom: 100px;">

        <div style="width: 1118px; height: 128px; display: block; padding: 0; vertical-align: top; margin-left: auto; margin-right: auto; margin-top: 30px;">
            <a href="<?php echo $this->GetBaseUrl();?>">
                <img src="<?php echo $this->GetBaseUrl();?>Content/favicon.ico" alt="Arma 3 Server Browser" height="128" width="128" />
            </a>
            <br />
            <div style="width: 100px; height: 1px"></div>
            <a href="https://plus.google.com/+ArmabrowserOrgofficial/posts" target="blank">
                <img src="<?php echo $this->GetBaseUrl();?>Content/Red-signin_Short_base_44dp.png" alt="Google + Armabrowser" height="44" width="44" />
            </a>
        </div>

        <div style="width: 900px; display: block; border-width: 1px; border-style:solid; border-color:#007acc; padding:40px; vertical-align:top; margin-left: auto; margin-right: auto; margin-top: -130px;">
            
            <ul class="navi">
                <li class="navi-item"><?php  echo Html::A( $this->GetBaseUrl().'Marves', 'Marves - Addon Repository'); ?></li>
                <li class="navi-item"><?php  echo Html::A( $this->GetBaseUrl().'downloadOnce.htm', 'Download 2016.1'); ?></li>
                <li class="navi-item"><?php  echo Html::A( $this->GetBaseUrl(), 'Arma browser'); ?></li>
            </ul>        
            <br />
            <?php 
                include($this->ViewPath) 
            ?>

        </div>
    </div>
    <div style="height: 100px; position: relative; z-index: 1; color:black; width: 800px; display: block; margin-left: auto; margin-right: auto;">
        <div>
            <p style="font-family:Verdana; color: black; font-size:10pt; text-align: center;">
                Arma 3 Server Browser is a launcher program for the game Arma 3. <br />It helps you to find Arma3 server without starting Arma 3 before.
                <br />
            </p>
            <p style="font-family:Verdana; color: black; font-size:10pt; text-align: center;">
                <a href="http://www.armabrowser.org/">armabrowser.org</a> ist keine kommerzielle Seite und erwirtschaftet keine Einnahmen. Das Programm ArmaBrowser ist frei und kann frei kopiert und genutzt werden. Kontakt: <img src="<?php echo $this->GetBaseUrl();?>Content/ArmaBrowseradd.png" alt="" />
            </p>
            <p style="font-family:Verdana; color: black; font-size:10pt; text-align: center;">
                <a href="http://www.armabrowser.org/">armabrowser.org</a> is a nonprofit web site. The program ArmaBrowser is free and can be copied and used freely. Contact: <img src="<?php echo $this->GetBaseUrl();?>Content/ArmaBrowseradd.png" alt="" />
            </p>
        </div>
    </div>
    <div>
        <p style="font-family:Verdana; color: black; font-size:8pt;">generated in <?php printf('%.4f',microtime(true) - $GLOBALS['StartMicrotime'])  ?> sec | <?php echo PHP_VERSION ?></p>
    </div>
</body>
</html>