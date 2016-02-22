-- phpMyAdmin SQL Dump
-- version 4.3.10
-- http://www.phpmyadmin.net
--
-- Host: localhost
-- Erstellungszeit: 03. Mrz 2015 um 22:15
-- Server-Version: 5.5.41-0ubuntu0.14.04.1
-- PHP-Version: 5.5.9-1ubuntu4.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;

--
-- Datenbank: arma
--

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle tbl_addon_keys
--

CREATE TABLE IF NOT EXISTS tbl_addon_keys (
  addon_keys_id int(10) unsigned NOT NULL,
  addon_key_tag varchar(45) CHARACTER SET latin1 NOT NULL,
  addon_key_pubkhash varchar(45) CHARACTER SET latin1 NOT NULL,
  addon_key_pubkh varchar(750) CHARACTER SET latin1 NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_german1_ci;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle tbl_installations
--

CREATE TABLE IF NOT EXISTS tbl_installations (
  installations_id int(10) unsigned NOT NULL,
  installations_nr char(36) NOT NULL,
  installations_created datetime NOT NULL,
  installations_lastsighting datetime NOT NULL,
  installations_authvalid tinyint(1) NOT NULL
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Tabellenstruktur für Tabelle tbl_installations_addons
--

CREATE TABLE IF NOT EXISTS tbl_installations_addons (
  installations_addons_id int(10) unsigned NOT NULL,
  installations_id int(10) unsigned NOT NULL,
  addon_keys_id int(10) unsigned NOT NULL,
  installations_addon_name varchar(45) NOT NULL,
  installations_addon_modname varchar(255) NOT NULL,
  installations_addon_displayname varchar(255) NOT NULL,
  installations_addon_version varchar(45) NOT NULL,
  installations_addon_lastsighting datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indizes der exportierten Tabellen
--

--
-- Indizes für die Tabelle tbl_addon_keys
--
ALTER TABLE tbl_addon_keys
  ADD PRIMARY KEY (addon_keys_id), ADD UNIQUE KEY uix_tbl_addon_keys_key__hash (addon_key_tag,addon_key_pubkhash) USING BTREE;

--
-- Indizes für die Tabelle tbl_installations
--
ALTER TABLE tbl_installations
  ADD PRIMARY KEY (installations_id), ADD KEY installations_nr (installations_nr);

--
-- Indizes für die Tabelle tbl_installations_addons
--
ALTER TABLE tbl_installations_addons
  ADD PRIMARY KEY (installations_addons_id), ADD KEY FK_tbl_installations_addons_installations_id (installations_id), ADD KEY FK_tbl_installations_addons_addon_keys_id (addon_keys_id);

--
-- AUTO_INCREMENT für exportierte Tabellen
--

--
-- AUTO_INCREMENT für Tabelle tbl_addon_keys
--
ALTER TABLE tbl_addon_keys
  MODIFY addon_keys_id int(10) unsigned NOT NULL AUTO_INCREMENT;
--
-- AUTO_INCREMENT für Tabelle tbl_installations
--
ALTER TABLE tbl_installations
  MODIFY installations_id int(10) unsigned NOT NULL AUTO_INCREMENT,AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT für Tabelle tbl_installations_addons
--
ALTER TABLE tbl_installations_addons
  MODIFY installations_addons_id int(10) unsigned NOT NULL AUTO_INCREMENT;
--
-- Constraints der exportierten Tabellen
--

--
-- Constraints der Tabelle tbl_installations_addons
--
ALTER TABLE tbl_installations_addons
ADD CONSTRAINT FK_tbl_installations_addons_addon_keys_id FOREIGN KEY (addon_keys_id) REFERENCES tbl_addon_keys (addon_keys_id),
ADD CONSTRAINT FK_tbl_installations_addons_installations_id FOREIGN KEY (installations_id) REFERENCES tbl_installations (installations_id);

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
