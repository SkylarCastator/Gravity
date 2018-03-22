var rotateSpeed = 25.0;
var speed = 50.0;
 
function Update() {
    var transAmount = speed * Time.deltaTime;
    var rotateAmount = rotateSpeed * Time.deltaTime;
    
    if (Input.GetKey("w")) {
    transform.Translate(0, 0, transAmount);	

    }

    if (Input.GetKey("a")) {
    transform.Rotate(0, -rotateAmount, 0);
    }
    if (Input.GetKey("d")) {
    transform.Rotate(0, rotateAmount, 0);
    }
    
    if (Input.GetKey ("q")) {
    transform.Rotate(0, 0, rotateAmount);
    }
            
}