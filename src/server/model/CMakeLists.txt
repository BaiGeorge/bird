#ADD_LIBRARY(model Price.cpp Order.cpp InstrumentManager.cpp PositionManager.cpp CashManager.cpp OrderManager.cpp InactiveOrderManager.cpp)
file(GLOB MODEL_SRC_FILES ${PROJECT_SOURCE_DIR}/model/ "*.cpp")
ADD_LIBRARY(model SHARED ${MODEL_SRC_FILES})
TARGET_LINK_LIBRARIES(model logger protomsg nanomsg boost_date_time)
