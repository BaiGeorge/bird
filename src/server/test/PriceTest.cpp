// #define BOOST_TEST_DYN_LINK
#define BOOST_TEST_MODULE "PriceModule"

#include <boost/test/included/unit_test.hpp>
#include "model/Price.h"
#include <iostream>

using namespace std;

BOOST_AUTO_TEST_CASE(testPrice)
{
  cout << "Begin" << endl;
  shared_ptr<Price> out;
  BOOST_CHECK(!out);
  {
    cout << "inner group begin" << endl;
    PricePtr p = std::make_shared<Price>();
    out = p;
    PricePtr p2 = std::make_shared<Price>();
    cout << "inner group end" << endl;
  }
  cout << "After inner gourp" << endl;
  BOOST_CHECK(out);
}
