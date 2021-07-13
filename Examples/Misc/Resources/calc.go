package main

import(
    "os/exec"
)

func main(){    
    c := exec.Command("calc") 
    c.Run()
}
// ASDASDASD