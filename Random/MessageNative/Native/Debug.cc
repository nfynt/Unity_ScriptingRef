#pragma once

#include "Debug.hh"


decltype(Debug::mode_)    Debug::mode_ = Debug::Mode::File;
decltype(Debug::logFunc_) Debug::logFunc_ = nullptr;
decltype(Debug::warnFunc_) Debug::warnFunc_ = nullptr;
decltype(Debug::errFunc_) Debug::errFunc_ = nullptr;
decltype(Debug::fs_)      Debug::fs_;
decltype(Debug::ss_)      Debug::ss_;
decltype(Debug::mutex_)   Debug::mutex_;


void Debug::Initialize()
{
    if (mode_ == Mode::File)
    {
        fs_.open("message.log");
        Debug::Log("Start");
    }
}


void Debug::Finalize()
{
    if (mode_ == Mode::File)
    {
        Debug::Log("Stop");
        fs_.close();
    }
}