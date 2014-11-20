using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSharing.FiniteFieldArithmetic
{
    public static class MathTools
    {
        public static bool IsPrime(double n)
        {
            if (n < 2)
                return false;
            if (n == 2) return true;
            double upTo = Math.Sqrt(n);
            for (int i = 3; i <= upTo; i += 2)
            {
                if (n % i == 0)
                    return false;
            }
            return true;
        }
        public static List<int> GeneratePrime3(int UpBound)
        {
            //potential prime
            var pp = 2;
            //prime set
            var ps = new List<int>();
            pp += 1;
            ps.Add(pp);
            while (pp < UpBound)
            {
                pp += 2;
                var test = true;
                var sqrtpp = Math.Sqrt(pp);
                foreach (var a in ps)
                {
                    if (a > sqrtpp) break;
                    if (pp % a == 0)
                    {
                        test = false;
                        break;
                    }
                }

                if (test) ps.Add(pp);
            }
            return ps;
        }
        public static List<int> GeneratePrime4(int UpBound)
        {

            //potential prime
            var pp = 2;

            //excepted prime set 
            //var ep = new List<int>(){pp};
            pp += 1;
             //test prime set
            var tp = new List<int>() { pp };
           // var ss = new List<int>() {2};

            while (pp < UpBound)
            {
                pp += 2;
                var test = true;
                var sqrtpp = Math.Sqrt(pp);
                foreach (var a in tp)
                {
                    if (a > sqrtpp) break;
                    if (pp % a == 0)
                    {
                        test = false;
                        break;
                    }
                }

                if (test) tp.Add(pp);
            }
            // ep.Reverse();
            return tp;//ep.Concat(tp).ToList();
        }
        public static List<int> GeneratePrime5(int lim)
        {
//            lim=raw_input("\nGenerate prime numbers up to what number? : ")
//""" Get an upper limit from the user to determine the generator's termination point. """
var sqrtlim=Math.Sqrt(lim);
//""" Get the square root of the upper limit. This will be the upper limit of the test prime array 
//for primes used to verify the primacy of any potential primes up to (lim). Primes greater than 
//(sqrtlim) will be placed in an array for extended primes, (xp), not needed for the verification 
//test. The use of an extended primes array is technically unnecessary, but helps to clarify that we 
//have minimized the size of the test prime array. """
var pp=2;
//""" Initialize the variable for the potential prime, setting it to begin with the first prime 
//number, (2). """
var ss= new List<int>(){pp};
//""" Initialize the array for the skip set, setting it at a single member, being (pp=2). Although 
//the value of the quantity of members in the skip set is never needed in the program, it may be 
//useful to understand that future skip sets will contain more than one member, the quantity of which 
//can be calculated, and is the quantity of members of the previous skip set multiplied by one less 
//than the value of the prime which the new skip set will exclude multiples of. Example - the skip 
//set which eliminates multiples of primes up through 3 will have (3-1)*1=2 members, since the 
//previous skip set had 1 member. The skip set which eliminates multiples of primes up through 5 will 
//have (5-1)*2=8 members, since the previous skip set had 2 members, etc. """
var ep=new List<int>(){pp};
//""" Initialize the array for primes which the skip set eliminate multiples of, setting the first 
//member as (pp=2) since the first skip set will eliminate multiples of 2 as potential primes. """
pp+=1;
//""" Advance to the first potential prime, which is 3. """
var rss=new List<int>(){ ss[0]};
//""" Initialize an array for the ranges of each skip set, setting the first member to be the range 
//of the first skip set, which is (ss[0]=2). Future skip sets will have ranges which can be 
//calculated, and are the sum of the members of the skip set. Another method of calculating the range 
//will also be shown below. """
var tp=new List<int>(){pp};
//""" Initialize an array for primes which are needed to verify potential primes against, setting the 
//first member as (pp=3), since we do not yet have a skip set that excludes multiples of 3. Also note 
//that 3 is a verified prime, without testing, since there are no primes less than the square root of 
//3. """
var i=0;
//""" Initialize a variable for keeping track of which skip set range is current. """
rss.Add(rss[i] * tp[0]);
//""" Add a member to the array of skip set ranges, the value being the value of the previous skip 
//set range, (rss[0]=2), multiplied by the value of the largest prime which the new skip set will 
//exclude multiples of, (tp[0]=3), so 2*3=6. This value is needed to define when to begin 
//constructing the next skip set. """
var xp=new List<int>();
//""" Initialize an array for extended primes which are larger than the square root of the user 
//defined limit (lim) and not needed to verify potential primes against, leaving it empty for now. 
//Again, the use of an extended primes array is technically unnecessary, but helps to clarify that we 
//have minimized the size of the test prime array. """
pp+=ss[0];
//""" Advance to the next potential prime, which is the previous potential prime, (pp=3), plus the 
//value of the next member of the skip set, which has only one member at this time and whose value is 
//(ss[0]=2), so 3+2=5. """
var npp=pp;
//""" Initialize a variable for the next potential prime, setting its value as (pp=5). """
tp.Add(npp);
//""" Add a member to the array of test primes, the member being the most recently identified prime, 
//(npp=5). Note that 5 is a verified prime without testing, since there are no TEST primes less than 
//the square root of 5. """
while (npp<lim){
//""" Loop until the user defined upper limit is reached. """
	i++;
    //""" Increment the skip set range identifier. """
	while (npp<rss[i]+1){
    //""" Loop until the next skip set range is surpassed, since data through that range is
    //needed before constructing the next skip set. """
		foreach(var n in ss){
        //""" Loop through the current skip set array, assigning the variable (n) the value 
        //of the next member of the skip set. """
			npp=pp+n;
            //""" Assign the next potential prime the value of the potential prime plus 
            //the value of the current member of the skip set. """
			if (npp>lim) break;
            //""" If the next potential prime is greater than the user defined limit, 
            //then end the 'for n' loop. """
			if (npp<=rss[i]+1) pp=npp;
            //""" If the next potential prime is still within the range of the next skip 
            //set, then assign the potential prime variable the value of the next 
            //potential prime. Otherwise, the potential prime variable is not changed 
            //and the current value remains the starting point for constructing the next 
            //skip set. """
			var sqrtnpp=Math.Sqrt(npp);
            //""" Get the square root of the next potential prime, which will be the 
            //limit for the verification process. """
			var test=true;
            //""" Set the verification flag to True. """
			foreach( var q in tp){
            //""" Loop through the array of the primes necessary for verification of the 
            //next potential prime. """
				if (sqrtnpp<q) break;
                //""" If the test prime is greater than the square root of the next 
                //potential prime, then end testing through the 'for q' loop. """
				else if( npp%q==0){
                //""" If the test prime IS a factor of the next potential prime. """
					test=false;
                    //""" Then set the verification flag to False since the next 
                    //potential prime is not a prime number. """
					break;
                }
                //    """ And end testing through the 'for q' loop. """
                //""" Otherwise, continue testing through the 'for q' loop. """
            }
			if (test){
            //""" If the next potential prime has been verified as a prime number. """
				if (npp<=sqrtlim) tp.Add(npp);
                //""" And if the next potential prime is less than or equal to the 
                //square root of the user defined limit, then add it to the array of 
                //primes which potential primes must be tested against. """
				else xp.Add(npp);
                }
            //    """ Otherwise, add it to the array of primes not needed to verify 
            //    potential primes against. """
            //""" Then continue through the 'for n' loop. """
		}
        if (npp>lim) break;
        //""" If the next potential prime is greater than the user defined limit, then end 
        //the 'while npp<rss[i]+1' loop. """
        //""" Otherwise, continue the 'while npp<rss[i]+1' loop. """
        }
	if (npp>lim) break;
    //""" If the next potential prime is greater than the user defined limit, then end the 'while 
    //npp<int(lim)' loop. """
    //""" At this point, the range of the next skip set has been reached, so we may begin
    //constructing a new skip set which will exclude multiples of primes up through the value of 
    //the first member of the test prime set, (tp[0]), from being selected as potential 
    //primes. """
	var lrpp=pp;
    //""" Initialize a variable for the last relevant potential prime and set its value to the 
    //value of the potential prime. """
	var nss=new List<int>();
    //""" Initialize an array for the next skip set, leaving it empty for now. """
	while( pp<(rss[i]+1)*2-1){
    //""" Loop until the construction of the new skip set has gone through the range of the new 
    //skip set. """
		foreach(var n in ss){
        //""" Loop through the current skip set array. """
			npp=pp+n;
            //""" Assign the next potential prime the value of the potential prime plus 
            //the value of the current member of the skip set. """
			if (npp>lim) break;
            //""" If the next potential prime is greater than the user defined limit, 
            //then end the 'for n' loop. """
			var sqrtnpp=Math.Sqrt(npp);
            //""" Get the square root of the next potential prime, which will be the 
            //limit for the verification process. """
			var test=true;
            //""" Set the verification flag to True. """
			foreach(var q in tp){
            //""" Loop through the array of the primes necessary for verification of the 
            //next potential prime. """
				if (sqrtnpp<q) break;
                //""" If the test prime is greater than the square root of the next 
                //potential prime, then end testing through the 'for q' loop. """
				else if( npp%q==0)
                //""" If the test prime IS a factor of the next potential prime. """
					test=false;
                    //""" Then set the verification flag to False since the next 
                    //potential prime is not a prime number. """
					break;
                //    """ And end testing through the 'for q' loop. """
                //""" Otherwise, continue testing through the 'for q' loop. """
            }
			if (test){
            //""" If the next potential prime has been verified as a prime number. """
				if (npp<=sqrtlim) tp.Add(npp);
                //""" And if the next potential prime is less than or equal to the 
                //square root of the user defined limit, then add it to the array of 
                //primes which potential primes must be tested against. """
				else xp.Add(npp);
                //""" Otherwise, add it to the array of primes not needed to verify 
                //potential primes against. """
            }
			if (npp%tp[0]!=0){
            //""" If the next potential prime was NOT factorable by the first member of 
            //the test array, then it is relevant to the construction of the new skip set 
            //and a member must be included in the new skip set for a potential prime to 
            //be selected. Note that this is the case regardless of whether the next 
            //potential prime was verified as a prime, or not. """
				nss.Add(npp-lrpp);
                //""" Add a member to the next skip set, the value of which is the 
                //difference between the last relevant potential prime and the next 
                //potential prime. """
				lrpp=npp;
                //""" Assign the variable for the last relevant potential prime the 
                //value of the next potential prime. """
            }
			pp=npp;
            //""" Assign the variable for the potential prime the value of the next 
            //potential prime. """
            //""" Then continue through the 'for n' loop. """
        }
		if (npp>lim) break;
        //""" If the next potential prime is greater than the user defined limit, then end 
        //the 'while npp<(rss[i]+1)*2-1' loop. """
        //""" Otherwise, continue the 'while npp<(rss[i]+1)*2-1' loop. """
    }
	if (npp>lim) break;
    //""" If the next potential prime is greater than the user defined limit, then end the 'while 
    //npp<int(lim)' loop. """
	ss=nss;
    //""" Assign the skip set array the value of the new skip set array. """
	ep.Add(tp[0]);
    //""" Add a new member to the excluded primes array, since the newly constructed skip set 
    //will exclude all multiples of primes through the first member of the test prime array. """
	tp.RemoveAt(0);
    //""" Delete the first member from the test prime array since future potential primes will 
    //not have to be tested against this prime. """
	rss.Add(rss[i]*tp[0]);
    //""" Add a member to the skip set range array with the value of the range of the next skip 
    //set. """
	npp=lrpp;
//    """ Assign the next potential prime the value of the last relevant potential prime. """
//    """ Then continue through the 'while npp<int(lim)' loop. """
}
    //""" At this point the user defined upper limit has been reached and the generator has completed 
//finding all of the prime numbers up to the user defined limit. """
ep.Reverse();
//""" Flip the array of excluded primes. """
            foreach (var a in ep)
	{
                tp.Insert(0,a);
	}
//[tp.insert(0,a) for a in ep]
//""" Add each member of the flipped array into the beginning of the test primes array. """
tp.Reverse();
//""" Flip the array of test primes. """
            foreach (var a in tp)
	{
                xp.Insert(0,a);
	}
//[xp.insert(0,a) for a in tp]
//""" Add each member of the flipped array into the beginning of the extended primes array. """
return xp;
//""" Send the completed array of all primes up to the user defined limit back to the function call. """
        }

        public static int[] Divisors(this double n)
        {
            int[] result = new int[0];

            for (int i = 1; i <= n; i++)
            {
                if (n % i == 0)
                {
                    Array.Resize(ref result, result.Length + 1);
                    result[result.Length - 1] = i;
                }
            }

            return result;
        }
    }
}
