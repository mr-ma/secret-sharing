#include "stdafx.h"
#include "Schoenmakers.h"
#include "NTL\ZZ_p.h"
#include "PrimeGenerator.h"
#include "NTL\ZZXFactoring.h"
#include "NonInteractiveChaumPedersen.h"
#include "NTLHelper.h"
#include <vector>
#include <tuple>
using namespace NTL;
using namespace std;
using namespace SecretSharingCore::ZKProtocols;
namespace SecretSharingCore
{
	namespace Algorithms{
		namespace PVSS{
			Schoenmakers::Schoenmakers(){
			}
			void Schoenmakers::SetPublicKeys(vector<ZZ_p> y){
				this->y =new vector<ZZ_p>(y);
			}
			void Schoenmakers::SetPublicKeys(List<array<Byte>^>^ y){
				vector<ZZ_p> vy;
				for (int i = 0; i < y->Count; i++)
				{
					vy.push_back(NTLHelper::ByteToZZ_p(y[i]));
				}
				this->SetPublicKeys(vy);
			}

			void Schoenmakers::SelectPrimeAndGenerators(int fieldSizeInByte){
	/*			PrimeGenerator^ pg = gcnew PrimeGenerator();
				this->fieldSizeInByte = fieldSizeInByte;
				this->q = new ZZ(173);
				ZZ_p::init(*this->q);
				this->g = new ZZ_p(31);
				this->G = new ZZ_p(pg->FindRandomGenerator(*this->q));*/

				this->fieldSizeInByte = fieldSizeInByte;
				// generate a random prime in the field
				PrimeGenerator^ pg = gcnew PrimeGenerator();
				ZZ sophiePrime = pg->GenerateZZSecureRandomPrime(fieldSizeInByte);
				this->q = new ZZ(sophiePrime * 2 +1);
				ZZ_p::init(*this->q);
				this->g = new ZZ_p(pg->FindRandomGenerator(*this->q));
				this->G =  new ZZ_p(pg->FindRandomGenerator(*this->q));
			}
			void Schoenmakers::Distribute(int t, int n, vector<ZZ_p>% EncryptedShares, vector<ZZ_p>% Commitments, vector<ZZ_p>% r, vector<ZZ_p>% c, ZZ_p% secret ){
				if (!y || ((vector<ZZ_p>)*y).size()<n ){
			/*		vector<ZZ_p> yobj = *y;
					if (yobj.empty() || yobj.size() < n){*/
						throw gcnew Exception("Not enough public key for participants defined");
					//}
				}
				
				ZZ_p::init(*this->q);
				ZZ_pX p;
				p.SetLength(t);
				
				// pick a polynomial of degree t-1 with random coefficients in Zq* 
				for (int j = 0; j < t; j++)
				{
					//alpha is going to the exponent so we have to use q-1
					ZZ_p::init(*q-1);
					ZZ_p alpha = random_ZZ_p();
					ZZ alphaz;
					//cout << "j:"<<j<<" alpha:"<<alpha << '\n';
					conv(alphaz, alpha);
					SetCoeff(p, j, alpha);
					ZZ_p::init(*q);
					// foreach coeff generate commitments C[j] = g^ alpha[j]
					ZZ_p commit = power(*this->g, alphaz);
					//cout << "commit:" << commit << '\n';
					Commitments.push_back(commit);
				}
				ZZ_p firstCoef;
				GetCoeff(firstCoef, p, 0);
				ZZ firstCoefz;
				conv(firstCoefz, firstCoef);
				//cout << "p(x) =" << p << '\n';
				secret = power(*G, firstCoefz);
				//cout << "secret:" << secret<< '\n';
				vector<ZZ_p> X;
				NonInteractiveChaumPedersen^ non_cp = gcnew NonInteractiveChaumPedersen(*q);

				//generate encrypted shares
				for (int i = 0; i < n; i++)
				{
					// p(i) is going to be used in exponents so q-1 is the field
					ZZ_p::init(*q-1);
					ZZ_p p_i = eval(p, ZZ_p(i+1));
					ZZ p_iz;
					conv(p_iz, p_i);
					ZZ_p::init(*q);
					//cout << "p(" << i + 1 << ")=" << p_i << " g:" << *g << " Xi= g ^ pi=" << power(*g, p_iz) << '\n';
					//revert back to the original field before non-exponential arithmetic
					
					ZZ_p y_i = ((vector<ZZ_p>)*y).at(i);
					ZZ_p Y_i = power(y_i, p_iz);

					//cout << "p(" << i + 1 << "):" << p_iz << " G ^ p(i):" << power(*G, p_iz) << " y:" << y_i << " Y:" << Y_i << '\n';
					EncryptedShares.push_back(Y_i);
					ZZ_p X_i = power(*g, p_iz);
					X.push_back(X_i);
					// generate the non-interactive proofs r,c using chaum pedersen algorithm
					// we wanna prove that we have an p(i) which  log _g -Xi = log _yi -Yi  
					ZZ_p r_i, c_i;
					non_cp->ComputeProofs(*g, y_i, X_i, Y_i, p_i, r_i, c_i);
					//cout << "i:" << i+1 <</* " yi:" << y_i <<*/ " Xi:" << X_i/* << " Y_i:" << Y_i << " r_i:" << r_i << " c_i:" << c_i */<< '\n';
					r.push_back(r_i);
					c.push_back(c_i);
				}
			
			}
			
			List<SchoenmakersShare^>^ Schoenmakers::Distribute(int t, int n, List<array<Byte>^>^% Commitments,array<Byte>^% secret){
				List<SchoenmakersShare^>^ shares = gcnew List<SchoenmakersShare^>();
				vector<ZZ_p> encryptedShares, commitments, c, r;
				ZZ_p sec;
				this->Distribute(t, n, encryptedShares, commitments, r, c,sec);
				for (int i = 0; i < n; i++)
				{
					SchoenmakersShare^ share = gcnew SchoenmakersShare(new ZZ_p( encryptedShares.at(i)),
						new ZZ_p(c.at(i)),new ZZ_p( r.at(i)));
					shares->Add(share);
				}
				for (int j = 0; j < t; j++)
				{
					array<Byte>^ commit =NTLHelper::ZZpToByte( commitments.at(j));
					Commitments->Add(commit);
				}
				secret = NTLHelper::ZZpToByte(sec);
				return shares;
			}

			List<SchoenmakersShare^>^ Schoenmakers::Distribute(int t, int n, array<Byte>^ sigma, List<array<Byte>^>^% Commitments, array<Byte>^% secret,array<Byte>^% U){
				ZZ_p::init(*this->q);
				if (sigma->Length*8 < 2){
					throw gcnew Exception("Sigma cannot be smaller than 2 bits");
				}
				List<SchoenmakersShare^>^ shares = this->Distribute(t, n, Commitments, secret);
				ZZ_p secretz = NTLHelper::ByteToZZ_p(secret);
				ZZ_p sigmaz = NTLHelper::ByteToZZ_p(sigma);
				ZZ_p::init(*this->q);
				ZZ_p Uz = sigmaz * secretz;
				//cout << "distribute sigma*G^s=U:" << Uz << " G^s: " << secretz << " sigma:" << sigmaz << '\n';
				U = NTLHelper::ZZpToByte(Uz);
				return shares;
			}
			bool Schoenmakers::VerifyDistributedShare(int i,ZZ_p r, ZZ_p c, ZZ_p Y, vector<ZZ_p> Commitments, ZZ_p y){
				ZZ_p::init(*this->q);
				// calculate X from commitments
				ZZ_p X_i = ZZ_p(1);
				for (int j = 0; j < Commitments.size(); j++)
				{
					ZZ jz = ZZ(j);
					ZZ iz = ZZ(i);
					
					ZZ expo = power(iz, j);
					//cout << "Commit" << j << ": " << Commitments.at(j)<<" exp:"<<expo<<'\n';
					X_i *= power(Commitments.at(j),expo);
				}
				//cout << "Verifying i:" << i /*<< " yi:" << y */<< " Xi:" << X_i /*<< " Y_i:" << Y << " r_i:" << r << " c_i:" << c*/<<'\n';
				NonInteractiveChaumPedersen^ ni_cp = gcnew NonInteractiveChaumPedersen(*q);
				return ni_cp->VerifyProofs(*g, y, X_i, Y, r, c);
			}
			bool Schoenmakers::VerifyDistributedShare(int i, SchoenmakersShare^ share, List<array<Byte>^>^ Commitments, array<Byte>^ y){
				ZZ_p::init(*this->q);
				vector<ZZ_p> commits;
				for (int j = 0; j < Commitments->Count; j++)
				{
					ZZ_p commit = NTLHelper::ByteToZZ_p(Commitments[j]);
					commits.push_back(commit);
				}
				return VerifyDistributedShare(i, *share->r, *share->c, *share->Y, commits, NTLHelper::ByteToZZ_p(y));
			}
			ZZ_p Schoenmakers::PoolShare(ZZ_p x, ZZ_p y, ZZ_p Y, ZZ_p% r, ZZ_p% c){
				// 1/x
				ZZ_p::init(*this->q-1);
				ZZ_p invX = inv(x);
				ZZ invXz;
				conv(invXz, invX);
				//div(invXz,ZZ(1), invXz);
				//ZZ_p::init(*this->q);
				ZZ_p::init(*this->q);
				ZZ_p S = power(Y, invXz);
				//cout << "x:" << x <<" 1/x:"<<invXz<< " y:"<<y<<" Y:"<<Y<<" S:" << S<<'\n';
				//make sure x was correct 
				ZZ xz;
				conv(xz, x);
				ZZ_p verifyY = power(S, xz);
				if (verifyY != Y) cout<<"decryption failed!";

				NonInteractiveChaumPedersen^ nicp = gcnew NonInteractiveChaumPedersen(*this->q);
				nicp->ComputeProofs(*this->G, S, y, Y, x, r, c);
				return S;

			} //r and c and proofs that party generate to prove he has the private key
			void Schoenmakers::PoolShare(Tuple< array<Byte>^, array<Byte>^ >^ keypair, SchoenmakersShare^% share){
				ZZ_p::init(*this->q);
				ZZ_p rshare, cshare;
				ZZ_p Si = this->PoolShare(NTLHelper::ByteToZZ_p(keypair->Item1), NTLHelper::ByteToZZ_p(keypair->Item2),
					*share->Y, rshare, cshare);
				share->proofc = new ZZ_p(cshare);
				share->proofr = new ZZ_p(rshare);
				share->S = new ZZ_p(Si);
			}

			bool Schoenmakers::VerifyPooledShares(int t, vector<ZZ_p> S,vector<ZZ_p> Y, vector<ZZ_p> r, vector<ZZ_p> c){
				
				NonInteractiveChaumPedersen^ nicp = gcnew NonInteractiveChaumPedersen(*this->q);
				
				int allVerified = 0;
				for (int i = 0; i < S.size(); i++)
				{
					bool verified = nicp->VerifyProofs(*this->G, S.at(i), (*y).at(i), Y.at(i), r.at(i), c.at(i));
					if (verified) allVerified++;
				}
				return allVerified >=t;
				
			}
			bool Schoenmakers::VerifyPooledShares(int t, List<SchoenmakersShare^>^ shares){
				vector<ZZ_p> S, Y,  r,  c;
				for (int i = 0; i < shares->Count; i++)
				{
					S.push_back(*shares[i]->S);
					Y.push_back(*shares[i]->Y);
					r.push_back(*shares[i]->proofr);
					c.push_back(*shares[i]->proofc);
				}
				return this->VerifyPooledShares(t, S, Y, r, c);
			}
			ZZ_p Schoenmakers::Reconstruct(int t,vector<ZZ_p> Shares){
				ZZ_p::init(*this->q);
				ZZ_p secret = ZZ_p(1);
				Vec<ZZ_p> y;
				Vec<int> x;
				for (int i = 0; i < Shares.size(); i++)
				{
					x.append( i+1);
					y.append(Shares.at(i));
					//cout << "x:" << i+1 << " y:" << y.at(i)<<'\n';
				}


				for (int i = 0; i < t; i++)                     //loop over all x inputs
				{
					float lambda = 1.0;
					for (int j = 0; j < t; j++)              //compute weights
					{
						if (i != j)
						{
							lambda = lambda * ((float)(- x.at(j)) / (float)(x.at(i) - x.at(j)));
						}

					}
					secret = secret * (power( y.at(i), (long)lambda));   //the interpolated function
				}
				//cout << "secret:"<<secret << '\n';
				return secret;
			}

			array<Byte>^ Schoenmakers::Reconstruct(int t, List<SchoenmakersShare^>^ Shares){
				vector<ZZ_p> S;
				for (int i = 0; i < Shares->Count; i++)
				{
					S.push_back(*Shares[i]->S);
				}
				return NTLHelper::ZZpToByte(this->Reconstruct(t, S));
			}
			array<Byte>^ Schoenmakers::Reconstruct(int t, List<SchoenmakersShare^>^ Shares, array<Byte>^ U){
				ZZ_p secretz = NTLHelper::ByteToZZ_p(this->Reconstruct(t, Shares));
				ZZ_p Uz = NTLHelper::ByteToZZ_p(U);
				ZZ_p sigma = Uz / secretz;
				//cout << "construct U:" << Uz << " G^s: " << secretz << " sigma:" << sigma << '\n';
				return NTLHelper::ZZpToByte(sigma);
			}
		
			ZZ Schoenmakers::Getq(){
				return *this->q;
			}
			ZZ_p Schoenmakers::Getg(){
				return *this->g;
			}
			ZZ_p Schoenmakers::GetG(){
				return  *this->G;
			}

			array<Byte>^ Schoenmakers::GetqB(){
				return NTLHelper::ZZToByte(*this->q);
			}
			array<Byte>^ Schoenmakers::GetgB(){
				return NTLHelper::ZZpToByte( *this->g);
			}
			array<Byte>^ Schoenmakers::GetGB(){
				return NTLHelper::ZZpToByte(*this->G);
			}
		}
	}
}
