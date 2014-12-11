#include "stdafx.h"
#include "Shamir.h"

namespace SecretSharing{
	namespace Shamir{
		Shamir::Shamir(){

		}
		List<IShare^>^ Shamir::DivideSecret(int K, int N, int Secret){

			//generate prime p
			ZZ p = GenGermainPrime_ZZ(primeLength);
			cout << "prime number is:" << p << '\n';

			ZZ_p::init(p);
			Vec<ZZ> coefficients = vec_ZZ();
			ZZ maxRandom = ZZ(coefficientLength);
			ZZ_pX f;
			f.SetLength(K);
			SetCoeff(f, 0, ZZ_p(Secret));
			for (int i = 1; i < K; i++)
			{
				ZZ r = RandomBnd(maxRandom);
				cout << "random coeff number is:" << r << '\n';
				SetCoeff(f, i, to_ZZ_p(r));
			}

			cout << "F is:" << f << '\n';
			List<IShare^>^ shares = gcnew List<IShare ^>();
			for (int i = 1; i <= N; i++)
			{
				ZZ_p yz = eval(f, ZZ_p(i));
				long y;
				conv(y, yz);
				ShamirShare^ sh = gcnew ShamirShare(i, y);
				shares->Add(sh);
			}

			return shares;
		}
		int Shamir::ReconstructSecret(List<IShare^>^ Shares){
			Vec<ZZ_p> y = vec_ZZ_p();
			Vec<ZZ_p> x = vec_ZZ_p();

			int count = Shares->Count;
			for (int i = 0; i < count; i++)
			{
				x.append(ZZ_p(Shares[i]->GetX()));
				ZZ_p yz;
				conv(yz, Shares[i]->GetY());
				y.append(yz);
			}
			ZZ_pX interpolatedf = interpolate(x, y);
			cout << "interpolated f:" << interpolatedf << '\n';

			ZZ_p secretz = eval(interpolatedf, ZZ_p(0));
			long secret;
			conv(secret, secretz);
			return secret;
		}
	}
}
