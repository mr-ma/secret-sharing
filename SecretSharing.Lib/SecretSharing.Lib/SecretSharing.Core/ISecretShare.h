
#pragma once
using namespace System;
using namespace System::Collections::Generic;

namespace SecretSharingCore
{
	namespace Common
	{
		public interface class IShare {

		public:
			int GetX();
			int GetY();
			String^ ToString();
		};

		
		public interface class ISecretShare {

		public:
			List<IShare^>^ DivideSecret(int K, int N, int Secret);
			int ReconstructSecret(List<IShare^>^ Shares);
		};
	}
}