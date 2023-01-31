# LupusecXT2PlusPlugin
Loupedeck Plugin for Lupusec XT2+

## Configuration
You need to create a .lupusecxt2plusplugin.json in your users folder.<br>
<br>Example:
```
C:\Users\username\.lupusecxt2plusplugin.json
```

It needs to have the following content

```json
{
  "uri": "https://theurltoyourlupusecxt2+",
  "ignorecertificationerrors": "1",
  "username": "youruser",
  "password": "youruserspassword"
  "smarthomegroups":
  [{
	  "name":"Group1",
	  "devices" : [{"name": "Lamp1"},{"name": "Switch1"},{"name": "Lamp2"}]
  },{
	  "name":"Group2",
	  "devices" : [{"name": "Lamp1"},{"name": "Switch1"},{"name": "Lamp2"}]
  },
  {
	  "name":"Group3",
	  "devices" : [{"name": "Lamp1"},{"name": "Switch1"},{"name": "Lamp2"}]
  }]}
```
The *password* will be encrypted after the first start of the plugin. Everytime you need to change it, just write it to the json file and it will be encypted after a restart of Loupedeck.<br>
If you have any official certificate you may deactivate *ignorecertificationerrors*. Just set it to 0 and your certificate will be checked.<br>
*username* and *password* are your Lupusec XT2+ login username and password.
##Smarthome
To control smarthome devices you need to define groups like "Group1" in the example and add the names of devices.