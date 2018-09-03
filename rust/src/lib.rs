fn identity<T> (me:T) -> T {
    me
}

fn compose<Fa:'static ,Fb: 'static,Ta,Tb,Tc> (fa : Fa, fb : Fb) -> Box<Fn(Ta) -> Tc>
where Fa: Fn(Ta) -> Tb, Fb : Fn(Tb) -> Tc
{
   return Box::new(move | a : Ta | fb(fa(a)));
}

#[cfg(test)]
mod tests {
    #[test]
    fn composition_preserves_identity() 
    {
        assert!(1==1)
    }
}
