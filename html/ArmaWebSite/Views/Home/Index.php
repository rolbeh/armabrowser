<?php 
require_once 'Classes/Html.php';
//include_once('Core/Exceptions.php');
//include_once('Core/Controller.php');

/**
 * @global Core\Controller Controller-Base class
 * @name $this
 */
 
if (!($this instanceof Core\Controller)) { throw new Exception("Invalid operation, expect class Controller"); }

?>
            <h1 class="title">Arma 3 Server Browser Version 0.8.0</h1>
            <span class="date"><?php echo date("F d, Y", strtotime($this->GetPageDate()))?></span>
            <div style="display: block; padding:40px; padding-top:0; vertical-align:top;">
                <p>Arma Browser is a launcher program for the game Arma 3. It helps you to find Arma III multiplayer server without starting Arma3 before and join your selected server with your installed addons.</p>
            </div>
            <div style="text-align: center;">
                <div style="margin-right: auto; margin-top: 10px; margin-bottom: 20px; border-width: 0px; border-style:solid; border-color:#688CAF;">
                    <a style="color:#827f00;font-size:16pt" href="<?php echo $this->GetBaseUrl(); ?>downloadOnce.htm">Download</a>
                    <p style="font-size:10pt; text-align: center;">updates supported</p>
                </div>
            </div>
            <div style="text-align: center;">
                <img src="Content/ArmaBrowser.png" alt="Arma Browser" />
            </div>
            <p>
                The target of ArmaBrowser is, find and join arma 3 server as fast as you can!
            </p>
            <div style="min-width: 200px; margin-left: auto; margin-right: auto; margin-top: 10px; margin-bottom: 20px;">
                <b>Features:</b>
                <ul>
                    <li>share and download small addons</li>

                    <li>auto joining when server is full</li>
                    <li>display the recently played server, after program start</li>
                    <li>find ArmA3 installation folder</li>
                    <li>shows the installed Addons and if possible more information</li>
                    <li>shows all running ArmA3 Server listed in Steam with the current player names</li>
                    <li>easy filter for server and missions</li>
                    <li>preselect available addons if the server provides informations</li>
                    <li>remember the last selected addons of servers</li>
                    <li>No administrator needed for installation or playing</li>
                </ul>

                <b>Changes:</b>
                <p>Version 0.8.0 <span style="font-family:Verdana; font-size:6pt; text-align: center; color:black;">18.08.2015</span></p>
                <ul>
                    <li>some improvements around addons</li>
                    <li>upload/share small addons (WEB server restrictions)</li>
                    <li>download addons</li>
                    <li>Userinterface improvements</li>
                </ul>

                <b>Changes:</b>
                <p>Version 0.7.3 <span style="font-family:Verdana; font-size:6pt; text-align: center; color:black;">24.06.2015</span></p>
                <ul>
                    <li>some improvements around addons</li>
                    <li>The BattlEye Service starts along with Arma 3</li>
                    <li>Userinterface improvements</li>
                </ul>

                <p>Version 0.7.0 <span style="font-family:Verdana; font-size:6pt; text-align: center; color:black;">18.03.2015</span></p>
                <ul>
                    <li>some improvements around Addons</li>
                    <li>First steps for an Addon manager</li>
                </ul>

                <p>Version 0.6.8</p>
                <ul>
                    <li>add auto-joining when server is full</li>
                    <li>some improvements while getting the serverlist</li>
                    <li>Fix: if no server is selected and starting Arma3 without joining server</li>
                </ul>
                <b>Requirements:</b>
                <ul>
                    <li>Installed Valve-Steam</li>
                    <li>Installed Arma 3 in Steam</li>
                    <li>Windows 7, 8, 8.1 or 10</li>
                    <li>.Net 4.5 Framework</li>
                </ul>
            </div>
            <div style="text-align: center;">
                <div style="margin-right: auto; margin-top: 10px; margin-bottom: 20px; border-width: 0px; border-style:solid; border-color:#688CAF;">
                    <a style="color:#827f00;font-size:16pt" href="/downloadOnce.htm">Download</a>
                    <p style="font-size:10pt; text-align: center;">updates supported</p>
                </div>
            </div>