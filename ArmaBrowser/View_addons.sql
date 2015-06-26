SELECT 
tbl_addon_keys.addon_key_pubkhash
,`client_addon_name`
,`client_addon_displayname`
,`client_addon_displaytext`
,`client_addon_version`
, MAX(client_addon_lastsighting) as client_addon_lastsighting
 FROM  tbl_client_addon 
INNER JOIN tbl_client ON tbl_client_addon.client_id=tbl_client.client_id
INNER JOIN tbl_client_addon_keys  ON tbl_client_addon.client_addon_id = tbl_client_addon_keys.client_addon_id
INNER JOIN tbl_addon_keys on tbl_client_addon_keys.addon_keys_id = tbl_addon_keys.addon_keys_id
 
group by 
`client_addon_name`
,`client_addon_displayname`
,`client_addon_displaytext`
,`client_addon_version`
,tbl_addon_keys.addon_key_pubkhash