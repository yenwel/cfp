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


#[cfg(test)]
mod tests {
    use std::ops::Neg;
    use std::thread;
    use std::time::Duration;

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
        assert!(f_addone(5)==f_addonememo(5));
        assert!(f_addone(5)==f_addonememo(5));        
    }
}
