use std::collections::HashMap;

pub fn identity<T> (me:T) -> T {
    me
}

pub fn compose<Fa:'static ,Fb: 'static,Ta,Tb,Tc> (fa : Fa, fb : Fb) -> Box<Fn(Ta) -> Tc>
where Fa: Fn(Ta) -> Tb, Fb : Fn(Tb) -> Tc
{
   return Box::new(move | a : Ta | fb(fa(a)));
}

pub fn memoize<Fa: 'static,Ta: 'static,Tb: 'static>(fa : Fa) -> Box<FnMut(Ta)->Tb> 
where Fa: Fn(Ta)->Tb, Ta: std::hash::Hash + std::cmp::Eq +  std::marker::Copy, Tb : std::marker::Copy
{
    let mut dict = HashMap::new();
    Box::new(move |a:Ta| {
        if dict.contains_key(&a) {
            let res = dict.get(&a).unwrap();
            *res
        } else {
            let fun_result = fa(a);
            dict.insert(a, fun_result);
            fun_result
        }        
    })
}

pub fn boolone(b: bool) -> bool
{
    b
}

pub fn booltwo(b: bool) -> bool
{
    !b
}

pub fn boolthree(_b: bool) -> bool
{
    true
}

pub fn boolfour(_b:bool) -> bool
{
    false
}


#[cfg(test)]
mod tests {
    extern crate rand;

    use std::ops::Neg;
    use std::thread;
    use std::time::{Duration, SystemTime};
    use self::rand::random;

    #[test]
    fn composition_preserves_identity() 
    {
        let f_one = super::compose(super::identity, i8::neg);
        let f_two = super::compose(i8::neg, super::identity);
        assert!(f_one(8)==f_two(8));
        assert!(f_one(-8)==f_two(-8));        
    }

     #[test]
    fn memoize_works() 
    {
        let f_addone = |a:  i8| {  thread::sleep(Duration::from_millis(100)); a + 1 };
        let mut f_addonememo = super::memoize(f_addone);
        let mut now = SystemTime::now(); 
        assert!(f_addone(5)==f_addonememo(5));
        // cargo test -- --nocapture
        println!("{:?}",now.elapsed().unwrap());
        now = SystemTime::now(); 
        assert!(f_addone(5)==f_addonememo(5));         
        println!("{:?}",now.elapsed().unwrap());
        let f_rand = |_: i64| {random::<i64>()};
        let mut f_randmemo = super::memoize(f_rand);
        now = SystemTime::now();
        assert!(f_randmemo(5)!=random::<i64>());               
        println!("{:?}",now.elapsed().unwrap());
    }
}
