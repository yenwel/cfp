pub fn identity<T> (me:T) -> T {
    me
}

pub fn compose<Fa:'static ,Fb: 'static,Ta,Tb,Tc> (fa : Fa, fb : Fb) -> Box<Fn(Ta) -> Tc>
where Fa: Fn(Ta) -> Tb, Fb : Fn(Tb) -> Tc
{
   return Box::new(move | a : Ta | fb(fa(a)));
}

#[cfg(test)]
mod tests {
    use std::ops::Neg;

    #[test]
    fn composition_preserves_identity() 
    {
        let f_one = super::compose(super::identity, i8::neg);
        let f_two = super::compose(i8::neg, super::identity);
        assert!(f_one(8)==f_two(8));
        assert!(f_one(-8)==f_two(-8));        
    }
}
