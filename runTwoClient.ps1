$Godot = "C:\Program Files\Godot\Godot.exe"


$Client1Args = '--position 0,0 -t $args'
Start-Process $Godot -ArgumentList $Client1Args

$Client2Args = '--position 1280,0 -t $args'
Start-Process $Godot -ArgumentList $Client2Args
