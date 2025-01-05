using UnityEngine;

public class HeroRotation : HeroAbility {
    [SerializeField] float rotationSpeed = 5;
    
    Vector3 direction;
    
    public void SetDirection(Vector3 direction) {
        this.direction = direction;
    }
    
    public override void Process() {
        if (direction != Vector3.zero) {
            var rot = Quaternion.LookRotation(direction);
            hero.Model.rotation = Quaternion.Slerp(hero.Model.rotation,
                rot, rotationSpeed * Time.deltaTime);
        }
    }
}