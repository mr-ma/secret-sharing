
#include <NTL/lzz_p.h>

#include <NTL/new.h>

NTL_START_IMPL

SmartPtr<zz_pInfoT> Build_zz_pInfo(FFTPrimeInfo *info)
{
   return MakeSmart<zz_pInfoT>(INIT_FFT, info);
}


zz_pInfoT::zz_pInfoT(long NewP, long maxroot)
{
   if (maxroot < 0) Error("zz_pContext: maxroot may not be negative");

   if (NewP <= 1) Error("zz_pContext: p must be > 1");
   if (NumBits(NewP) > NTL_SP_NBITS) Error("zz_pContext: modulus too big");

   ZZ P, B, M, M1, MinusM;
   long n, i;
   long q, t;

   p = NewP;

   pinv = 1/double(p);

   p_info = 0;
   p_own = 0;

   conv(P, p);

   sqr(B, P);
   LeftShift(B, B, maxroot+NTL_FFTFudge);

   set(M);
   n = 0;
   while (M <= B) {
      UseFFTPrime(n);
      q = GetFFTPrime(n);
      n++;
      mul(M, M, q);
   }

   if (n > 4) Error("zz_pInit: too many primes");

   NumPrimes = n;
   PrimeCnt = n;
   MaxRoot = CalcMaxRoot(q);

   if (maxroot < MaxRoot)
      MaxRoot = maxroot;

   negate(MinusM, M);
   MinusMModP = rem(MinusM, p);

   if (!(CoeffModP = (long *) NTL_MALLOC(n, sizeof(long), 0)))
      Error("out of space");

   if (!(x = (double *) NTL_MALLOC(n, sizeof(double), 0)))
      Error("out of space");

   if (!(u = (long *) NTL_MALLOC(n, sizeof(long), 0)))
      Error("out of space");

   for (i = 0; i < n; i++) {
      q = GetFFTPrime(i);

      div(M1, M, q);
      t = rem(M1, q);
      t = InvMod(t, q);
      if (NTL_zz_p_QUICK_CRT) mul(M1, M1, t);
      CoeffModP[i] = rem(M1, p);
      x[i] = ((double) t)/((double) q);
      u[i] = t;
   }
}

zz_pInfoT::zz_pInfoT(INIT_FFT_TYPE, FFTPrimeInfo *info)
{
   p = info->q;
   pinv = info->qinv;

   p_info = info;
   p_own = 0;

   NumPrimes = 1;
   PrimeCnt = 0;

   MaxRoot = CalcMaxRoot(p);
}

// FIXME: we could make bigtab an optional argument

zz_pInfoT::zz_pInfoT(INIT_USER_FFT_TYPE, long q)
{
   long w;
   if (!IsFFTPrime(q, w)) Error("invalid user supplied prime");

   p = q;
   pinv = 1/((double) q);

   p_info = NTL_NEW_OP FFTPrimeInfo();
   if (!p_info) Error("out of memory");

   long bigtab = false;
#ifdef NTL_FFT_BIGTAB
   bigtab = true;
#endif
   InitFFTPrimeInfo(*p_info, q, w, bigtab); 

   p_own = 1;

   NumPrimes = 1;
   PrimeCnt = 0;

   MaxRoot = CalcMaxRoot(p);
}



zz_pInfoT::~zz_pInfoT()
{
   if (!p_info) {
      free(CoeffModP);
      free(x);
      free(u);
   }
   else {
      if (p_own) delete p_info;
   }
}


NTL_THREAD_LOCAL SmartPtr<zz_pInfoT> zz_pInfo = 0;



void zz_p::init(long p, long maxroot)
{
   zz_pContext c(p, maxroot);
   c.restore();

}

void zz_p::FFTInit(long index)
{
   zz_pContext c(INIT_FFT, index);
   c.restore();
}

void zz_p::UserFFTInit(long q)
{
   zz_pContext c(INIT_USER_FFT, q);
   c.restore();
}

zz_pContext::zz_pContext(long p, long maxroot) : 
   ptr(MakeSmart<zz_pInfoT>(p, maxroot)) 
{ }

zz_pContext::zz_pContext(INIT_FFT_TYPE, long index)
{
   if (index < 0)
      Error("bad FFT prime index");

   UseFFTPrime(index);

   ptr =  FFTTables[index]->zz_p_context;
}

zz_pContext::zz_pContext(INIT_USER_FFT_TYPE, long q) :
   ptr(MakeSmart<zz_pInfoT>(INIT_USER_FFT, q))
{ }


void zz_pContext::save()
{
   ptr = zz_pInfo;
}

void zz_pContext::restore() const
{
   zz_pInfo = ptr;
}



zz_pBak::~zz_pBak()
{
   if (MustRestore) c.restore();
}

void zz_pBak::save()
{
   c.save();
   MustRestore = true;
}


void zz_pBak::restore()
{
   c.restore();
   MustRestore = false;
}




static inline 
long reduce(long a, long p)
{
   if (a >= 0 && a < p)
      return a;
   else {
      a = a % p;
      if (a < 0) a += p;
      return a;
   }
}


zz_p to_zz_p(long a)
{
   return zz_p(reduce(a, zz_p::modulus()), INIT_LOOP_HOLE);
}

void conv(zz_p& x, long a)
{
   x._zz_p__rep = reduce(a, zz_p::modulus());
}

zz_p to_zz_p(const ZZ& a)
{
   return zz_p(rem(a, zz_p::modulus()), INIT_LOOP_HOLE);
}

void conv(zz_p& x, const ZZ& a)
{
   x._zz_p__rep = rem(a, zz_p::modulus());
}


istream& operator>>(istream& s, zz_p& x)
{
   NTL_ZZRegister(y);
   NTL_INPUT_CHECK_RET(s, s >> y);
   conv(x, y);

   return s;
}

ostream& operator<<(ostream& s, zz_p a)
{
   NTL_ZZRegister(y);
   y = rep(a);
   s << y;

   return s;
}

NTL_END_IMPL
