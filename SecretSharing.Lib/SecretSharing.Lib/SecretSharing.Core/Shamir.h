#include "ISecretShare.h"
#include <NTL/ZZ_pXFactoring.h>
#pragma once

using namespace System::Collections::Generic;
using namespace NTL;
using namespace std;
using namespace SecretSharing::Common;

namespace SecretSharing{
	namespace Shamir{
		ref class ShamirShare :IShare
		{
		private:
			int _x;
			int _y;
		public:
			ShamirShare(const ShamirShare% rhs){
				_x = rhs._x;
				_y = rhs._y;
			}
			const ShamirShare operator=(const ShamirShare% rhs){
				_x = rhs._x;
				_y = rhs._y;
				return *this;
			}

			ShamirShare::ShamirShare(int x, int y){
				_x = x;
				_y = y;
			}
			virtual int GetX(){
				return _x;
			}
			virtual int GetY(){
				return _y;
			}
			virtual String^ ToString() override
			{
				return _x.ToString();
			}
		};


		ref class Shamir :ISecretShare{
		private:
			const long primeLength = 18;
			const long coefficientLength = 59123;
		public:
			Shamir::Shamir();
			virtual List<IShare^>^ Shamir::DivideSecret(int K, int N, int Secret);
			virtual int Shamir::ReconstructSecret(List<IShare^>^ Shares);
		};
	}

}