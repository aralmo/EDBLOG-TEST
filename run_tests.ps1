$folders = Get-ChildItem ./src/*.unittests
foreach($f in $folders){
    $fn = './src/{0}' -f $f.Name
    dotnet test -v minimal $fn
}