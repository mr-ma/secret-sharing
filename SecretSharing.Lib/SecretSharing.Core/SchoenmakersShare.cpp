#include "stdafx.h"
#include "SchoenmakersShare.h"

namespace SecretSharingCore
{
	namespace Algorithms{
		namespace PVSS{
			SchoenmakersShare::SchoenmakersShare(ZZ_p* Y, ZZ_p* c, ZZ_p* r){
				this->c = c;
				this->r = r;
				this->Y = Y;
			}
			bool SchoenmakersShare::IsPooled(){
				return (this->S && this->proofc && this->proofr);
			}
		}
	}
}