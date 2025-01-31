#ifndef BASE_LOGSTREAM_H
#define BASE_LOGSTREAM_H

#include <assert.h>
#include <string.h> // memcpy
#include <string>
#include "boost/noncopyable.hpp"
#include "boost/format/format_class.hpp"
#include "StringPiece.h"

namespace base {

namespace detail {

const int kSmallBuffer = 4000;
const int kLargeBuffer = 4000*1000;

template<int SIZE>
class FixedBuffer : boost::noncopyable {
 public:
  FixedBuffer() : cur_(data_) {}

  ~FixedBuffer() {}

  void append(const char* /*restrict*/ buf, size_t len) {
    // FIXME: append partially
    size_t left = implicit_cast<size_t>(avail());
    if (left > len) {
      memcpy(cur_, buf, len);
      cur_ += len;
    } else {
      const int ELLIPSIS_LENGTH = 6;
      if (left > ELLIPSIS_LENGTH ) {
        memcpy(cur_, "......", ELLIPSIS_LENGTH );
        cur_ += ELLIPSIS_LENGTH;
      }
      // stop(); /// write all buffer
    }
  }

  const char* data() const { return data_; }
  int length() const { return static_cast<int>(cur_ - data_); }

  // write to data_ directly
  char* current() { return cur_; }
  int avail() const { return static_cast<int>(end() - cur_); }
  void add(size_t len) { cur_ += len; }
  void stop() { cur_ = data_ + sizeof data_; }

  void reset() { cur_ = data_; }
  void bzero() { ::bzero(data_, sizeof data_); }

  // for used by GDB
  const char* debugString();
  // for used by unit test
  std::string toString() const { return std::string(data_, length()); }
  StringPiece toStringPiece() const { return StringPiece(data_, length()); }

 private:
  const char* end() const { return data_ + sizeof data_; }

  char data_[SIZE];
  char* cur_;
};

} // namespace detail

class LogStream : boost::noncopyable {
  typedef LogStream self;
 public:
  typedef detail::FixedBuffer<detail::kSmallBuffer> Buffer;

  self& operator<<(bool v) {
    buffer_.append(v ? "1" : "0", 1);
    return *this;
  }

  self& operator<<(short);
  self& operator<<(unsigned short);
  self& operator<<(int);
  self& operator<<(unsigned int);
  self& operator<<(long);
  self& operator<<(unsigned long);
  self& operator<<(long long);
  self& operator<<(unsigned long long);

  self& operator<<(const void*);

  self& operator<<(float v) {
    *this << static_cast<double>(v);
    return *this;
  }
  self& operator<<(double);
  // self& operator<<(long double);

  self& operator<<(char v) {
    buffer_.append(&v, 1);
    return *this;
  }

  // self& operator<<(signed char);
  // self& operator<<(unsigned char);

  self& operator<<(const char* str) {
    if (str) {
      buffer_.append(str, strlen(str));
    } else {
      buffer_.append("(null)", 6);
    }
    return *this;
  }

  self& operator<<(const unsigned char* str) {
    return operator<<(reinterpret_cast<const char*>(str));
  }

  self& operator<<(const sso_string& v) {
    buffer_.append(v.c_str(), v.size());
    return *this;
  }

  self& operator<<(const std::string& v) {
    buffer_.append(v.c_str(), v.size());
    return *this;
  }

  self& operator<<(const StringPiece& v) {
    buffer_.append(v.data(), v.size());
    return *this;
  }

  template<class Ch, class Tr, class Alloc>
  self& operator<<(boost::basic_format<Ch, Tr, Alloc> &fmt) {
    return operator<<(fmt.str());
  }

  self& operator<<(const Buffer& v) {
    *this << v.toStringPiece();
    return *this;
  }

  void append(const char* data, int len) { buffer_.append(data, len); }
  const Buffer& buffer() const { return buffer_; }
  void resetBuffer() { buffer_.reset(); }

 private:
  void staticCheck();

  template<typename T> void formatInteger(T);

  Buffer buffer_;
  static const int kMaxNumericSize = 32;
};

class Fmt : boost::noncopyable {
 public:
  template<typename T> Fmt(const char* fmt, T val);

  const char* data() const { return buf_; }
  int length() const { return length_; }

 private:
  char buf_[32];
  int length_;
};

inline LogStream& operator<<(LogStream& s, const Fmt& fmt) {
  s.append(fmt.data(), fmt.length());
  return s;
}

} // namespace base

#endif  // BASE_LOGSTREAM_H

