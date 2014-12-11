#include "ISecretShare.h"
#pragma once

using namespace SecretSharing::Common;
public ref class ShamirShare :IShare
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