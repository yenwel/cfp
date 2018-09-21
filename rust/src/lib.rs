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

type PartialFunc<Tin,Tout> = Fn(Tin) -> Option<Tout>; // generic type alias!!!

fn identity_partialfn<Tin :'static,Tout:'static>(pf: Box<PartialFunc<Tin,Tout>>) -> Box<PartialFunc<Tin,Tout>>
{
    Box::new(move |tin: Tin| pf(tin))
}

fn compose_partialfn<Ta : 'static,Tb: 'static,Tc:'static>(pf_one: Box<PartialFunc<Ta,Tb>>, pf_two: Box<PartialFunc<Option<Tb>,Tc>>) -> Box<PartialFunc<Ta,Tc>> { 
    
     return Box::new(move | a : Ta | pf_two(pf_one(a)));
}

fn safe_root (x : Option<f64> ) -> Option<f64> {
    if let Some(num) = x{
        if num >= 0.0 {
             return Some(num.sqrt());
        }
    }
    None
}

fn safe_reciprocal (x: Option<f64>) -> Option<f64> {
    if let Some(num) = x {
        if num != 0.0 {
            return Some(1.0 / num);
        }
    }
    None
}

fn safe_root_reciprocal( x: Option<f64>) -> Option<f64> {
    return compose_partialfn(Box::new(safe_root),Box::new(safe_reciprocal))(x);
}

#[cfg(test)]
mod tests {
    extern crate rand;

    use std::ops::Neg;
    use std::thread;
    use std::time::{Duration, SystemTime};
    use self::rand::{random, Rng, SeedableRng, StdRng};
    use std::iter;
    use std::hash::Hash;
    use std::cmp::Eq;
    use std::marker::Copy;
    use std::cmp::PartialEq;

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
    }

    #[test]
    fn memoize_random_does_not_work() 
    {  
        let f_rand = |_: i64| {random::<i64>()};
        let mut f_randmemo = super::memoize(f_rand);
        let now = SystemTime::now(); 
        assert!(f_randmemo(5)!=f_rand(5));               
        println!("{:?}",now.elapsed().unwrap());
    }
    
    #[test]
    fn memoize_random_seeded_works() 
    {          
        let f_rand_seeded = |seed: [u8;32]| 
        {                     
            let mut rng: StdRng = SeedableRng::from_seed(seed);
            rng.gen::<f64>()
        };
        let input_vec: Vec<u8> = iter::repeat(42).take(32).collect();     
        let mut input = [0; 32];
        input.copy_from_slice(&input_vec);
        let mut f_rand_seeded_memo = super::memoize(f_rand_seeded);
        let mut now = SystemTime::now();
        assert!(f_rand_seeded_memo(input)==f_rand_seeded(input));                       
        println!("{:?}",now.elapsed().unwrap());            
        now = SystemTime::now();        
        assert!(f_rand_seeded_memo(input)==f_rand_seeded(input));     
        println!("{:?}",now.elapsed().unwrap());
    }

    fn assert_purity_for_input<Fa: 'static,Ta: 'static,Tb: 'static>(f : Fa, input: Ta, is_pure: bool)
    where Fa: Fn(Ta)->Tb + Copy, Ta: Hash + Eq + Copy, Tb : Copy + PartialEq
    {
        let mut f_memo = super::memoize(f);
        let mut now = SystemTime::now();
        assert!((f_memo(input) == f(input)) == is_pure);                       
        println!("{:?}",now.elapsed().unwrap());            
        now = SystemTime::now();    
        assert!((f_memo(input) == f(input)) == is_pure);      
        println!("{:?}",now.elapsed().unwrap());
    }

    #[test]
    fn factorial_is_pure() 
    {          
        assert_purity_for_input(
            |n : u128|
            {
                let mut result = 1;
                for i in 2..n {result *= i;}
                result
            },
            21, //otherwise overflow lol
            true
        )
    }

    #[test]
    fn return_true_is_pure() 
    {          
        assert_purity_for_input(
            |_ :()|
            {
                println!("Hello!");
                true
            },
            (),
            true
        )
    }

    /*#[test]
    fn increment_with_static_in_closure_scope_is_impure() 
    {
        let mut y = 0;
        assert_purity_for_input(
            |x|
            {
                y +=x;
                y
            },
            42,
            false
        )
    }*/

   
}
