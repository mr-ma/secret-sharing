#include "ISecretShare.h"
#pragma once

using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
namespace SecretSharingCore
{
	namespace Common
	{
		public interface class IShareCollection: IDisposable
		{
		public:
			int GetCount();
			List<IShare^>^ GetAllShares();
			IShare^ GetShare(int index);
			void SetShare(int index, IShare^ share);
			String^ ToString();
		};


		public ref class ShareCollection :IShareCollection{
		private:
		public:
			virtual int ShareCollection::GetCount();
			virtual List<IShare^>^ ShareCollection::GetAllShares();
			virtual IShare^ ShareCollection::GetShare(int index);
			virtual void ShareCollection::SetShare(int index, IShare^ share);
			static  void ScatterShareIntoCollection(List<IShare^>^ shares, List<IShareCollection^>^ currentCollection, int index);
			static List<IShare^>^ GatherShareFromCollection(List<IShareCollection^>^ currentCollection, int i);
			virtual String^ ToString() override;
			ShareCollection::ShareCollection();
			ShareCollection::!ShareCollection();
		protected:
			ShareCollection::~ShareCollection();
		internal:
			List<IShare^>^ shares;
		};
	}
}