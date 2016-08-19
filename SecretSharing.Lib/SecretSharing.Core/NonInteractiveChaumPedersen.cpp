#include "stdafx.h"
#include "NonInteractiveChaumPedersen.h"
#include "NTLHelper.h"
#include "NTL\ZZ_p.h"
#include "NTL\ZZ_pE.h"
#include "NTL\mat_ZZ_p.h"
using namespace System::Security::Cryptography;
using namespace System::Linq;
using namespace std;
namespace SecretSharingCore
{
	namespace ZKProtocols{

		NonInteractiveChaumPedersen::NonInteractiveChaumPedersen(array<Byte>^ PrimeFieldInBytes){
			this->q =new ZZ( NTLHelper::ByteToZZ(PrimeFieldInBytes));
		}
		NonInteractiveChaumPedersen::NonInteractiveChaumPedersen(ZZ PrimeField){
			this->q =new ZZ( PrimeField);
		}
		void NonInteractiveChaumPedersen::ComputeProofs(array<Byte>^ Base1, array<Byte>^ Base2, 
			array<Byte>^ Result1, array<Byte>^ Result2, array<Byte>^ Secret, array<Byte>^% R,array<Byte>^% C){
				//prepare values
				ZZ_p::init(*this->q);
				ZZ_p base1 = NTLHelper::ByteToZZ_p(Base1);
				ZZ_p base2 = NTLHelper::ByteToZZ_p(Base2);
				ZZ_p result1 = NTLHelper::ByteToZZ_p(Result1);
				ZZ_p result2 = NTLHelper::ByteToZZ_p(Result2);
				ZZ_p secret = NTLHelper::ByteToZZ_p(Secret);
				ZZ_p c,r;
				ComputeProofs(base1,base2,result1,result2,secret,r,c);
				C = NTLHelper::ZZpToByte(c);
				R= NTLHelper::ZZpToByte(r);
		}

		bool NonInteractiveChaumPedersen::VerifyProofs(array<Byte>^ Base1, array<Byte>^ Base2, 
			array<Byte>^ Result1, array<Byte>^ Result2, array<Byte>^ R,array<Byte>^ C){
				//prepare values
				ZZ_p::init(*this->q);
				ZZ_p base1 = NTLHelper::ByteToZZ_p(Base1);
				ZZ_p base2 = NTLHelper::ByteToZZ_p(Base2);
				ZZ_p result1 = NTLHelper::ByteToZZ_p(Result1);
				ZZ_p result2 = NTLHelper::ByteToZZ_p(Result2);
				ZZ_p c = NTLHelper::ByteToZZ_p(C);
				ZZ_p r = NTLHelper::ByteToZZ_p(R);
				return VerifyProofs(base1,base2,result1,result2,r,c);
				
		}

		void NonInteractiveChaumPedersen::ComputeProofs(ZZ_p Base1, ZZ_p Base2,
			ZZ_p Result1, ZZ_p Result2, ZZ_p Secret, ZZ_p% R, ZZ_p% C){
			
			//w is exponent so must be in field of q-1
			ZZ_p::init(*this->q-1);
			//generate a random w in the field of q-1
			ZZ_p w =  random_ZZ_p();//ZZ_p(15);
			ZZ wz;
			conv(wz,w);
			/*Console::WriteLine("w{0}", NTLHelper::ZZToByte(wz)[0]);*/
			//switch back to the q field
			ZZ_p::init(*this->q);
			//compute a1= base1 ^ w and a2= base2 ^ w
			ZZ_p a1 = power(Base1, wz);
			ZZ_p a2 = power(Base2, wz);


			//cout<<"w:"<<w << " a1:"<<a1 <<" a2:"<<a2<<'\n';

			array<Byte>^ A1 = NTLHelper::ZZpToByte(a1);
			array<Byte>^ A2 = NTLHelper::ZZpToByte(a2);
			//Console::WriteLine("a1:{0}, a2:{1}", A1[0],A2[0]);

			//switch prime field to calculate exponents
			ZZ_p::init(*q-1);

			//compute c as a hash(result1,result2,a1,a2) like this we are binding c to a and 
			//preventing manipulation of c from the prover
			array<Byte>^ c = ComputeHash(NTLHelper::ZZpToByte(Result1), NTLHelper::ZZpToByte(Result2), A1, A2);

			
			C = NTLHelper::ByteToZZ_p(c);
			/*long ci;
			conv(ci, C);
			Console::WriteLine("C:{0}", ci);*/
			
			ZZ_p r = w - Secret*C;
			R = r;
			/*cout<<"r:"<<r<<" R:"<<R<<'\n';
			long ri;
			conv(ri, r);
			Console::WriteLine("R:{0}", ri);*/
			
			//switch prime field to initial one
			//ZZ_p::init(*q);
		}

		bool NonInteractiveChaumPedersen::VerifyProofs(ZZ_p Base1, ZZ_p Base2,
			ZZ_p Result1, ZZ_p Result2, ZZ_p r, ZZ_p c){
			//prepare values
			ZZ_p::init(*this->q);


			ZZ zc;
			ZZ zr;
			conv(zc, c);
			conv(zr, r);

			/*long zci;
			conv(zci, zc);
			long zri;
			conv(zri, zr);
			long base1i;
			conv(base1i, Base1);
			long result1i;
			conv(result1i, Result1);
			Console::WriteLine("input Base1:{0}^R:{1} * Result1:{2} C:{3}, ",base1i,zri,result1i,zci);*/

			//compute na1= (base1 ^ r)(result1 ^ c)  and na2=( base2 ^ r)(result2 ^ c)
			//ZZ_p tmp1 = power(Base1, zr);
			//ZZ_p tmp2 = power(Result1,zc); // power(Result1, zc);
			//long tmp1i;
			//conv(tmp1i, tmp1);
			//long tmp2i;
			//conv(tmp2i, tmp2);
			//Console::WriteLine("calc tmp1:{0}, tmp2:{1}", tmp1i,tmp2i);
			ZZ_p na1 = power(Base1, zr)*power(Result1,zc);
			ZZ_p na2 = power(Base2, zr) * power(Result2, zc);

			//cout<<"proof c:"<<c<<" r:"<<r<< " na1:"<<na1<<" na2:"<<na2<<'\n';

			array<Byte>^ NA1 = NTLHelper::ZZpToByte(na1);
			array<Byte>^ NA2 = NTLHelper::ZZpToByte(na2);
			/*long na1i;
			conv(na1i, na1);
			long na2i;
			conv(na2i, na2);
			Console::WriteLine("na1:{0}, na2:{1}", na1i,na2i);*/

			//compute once again c
			array<Byte>^ NC = ComputeHash(NTLHelper::ZZpToByte( Result1),NTLHelper::ZZpToByte( Result2), NA1, NA2);
			
			//switch prime field to calculate exponents
			ZZ_p::init(*q-1);
			ZZ_p nc = NTLHelper::ByteToZZ_p(NC);
			/*long ci;
			conv(ci, nc);
			Console::WriteLine("Hashed result C:{0}", ci);*/

			//compare NC equals C
			if(nc!=c){
				cout<<"failed!";
			}
			return nc==c;
		}

		array<Byte>^ NonInteractiveChaumPedersen::ComputeHash(array<Byte>^ Result1, array<Byte>^ Result2,array<Byte>^ A1, array<Byte>^ A2){
			// Initialize a SHA256 hash object.
			SHA256 ^ mySHA256 = SHA256Managed::Create();
			array<Byte>^ rv = gcnew array<Byte>( Result1->Length + Result2->Length + A1->Length + A2->Length );
			System::Buffer::BlockCopy( Result1, 0, rv, 0, Result1->Length );
			System::Buffer::BlockCopy( Result2, 0, rv, Result1->Length, Result2->Length );
			System::Buffer::BlockCopy( A1, 0, rv, Result1->Length + Result2->Length, A1->Length );
			System::Buffer::BlockCopy( A2, 0, rv, Result1->Length + Result2->Length+A1->Length, A2->Length);
			array<Byte>^ hashValue = mySHA256->ComputeHash(rv);
			return hashValue;
		}
	}
}