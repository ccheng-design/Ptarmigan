cmake_minimum_required(VERSION 3.16 FATAL_ERROR)
set(CMAKE_CXX_STANDARD ${MR_CXX_STANDARD})
set(CMAKE_CXX_STANDARD_REQUIRED ON)

project(mrmeshnumpy CXX)

file(GLOB SOURCES "*.cpp")
file(GLOB HEADERS "*.h")

add_library(${PROJECT_NAME} SHARED ${SOURCES} ${HEADERS})

set_target_properties(${PROJECT_NAME} PROPERTIES
  PREFIX ""
  LIBRARY_OUTPUT_DIRECTORY "${CMAKE_LIBRARY_OUTPUT_DIRECTORY}/meshlib"
)

# This allows us to share bindings for C++ types across compilers (across GCC and Clang). Otherwise Pybind refuses
# to share them because the compiler name and the ABI version number are different, even when there's no actual ABI incompatibility in practice.
add_compile_definitions(PYBIND11_COMPILER_TYPE=\"_meshlib\")
add_compile_definitions(PYBIND11_BUILD_ABI=\"_meshlib\")

if(APPLE)
  set(CMAKE_SHARED_LIBRARY_SUFFIX ".so")
  target_link_libraries(${PROJECT_NAME} PRIVATE
    MRMesh
    MRPython
    spdlog
    tbb
    pybind11::module
  )
ELSE()
  target_link_libraries(${PROJECT_NAME} PRIVATE
    MRMesh
    MRPython
    spdlog
    tbb
    Python::Python
  )
ENDIF()

IF(MESHLIB_BUILD_VOXELS)
  target_link_libraries(${PROJECT_NAME} PRIVATE
    MRVoxels
  )
ELSE()
  target_compile_definitions(${PROJECT_NAME} PRIVATE MESHLIB_NO_VOXELS)
ENDIF()

install(TARGETS ${PROJECT_NAME} DESTINATION "${MR_PY_LIB_DIR}")

IF(MR_PCH)
  TARGET_PRECOMPILE_HEADERS(${PROJECT_NAME} REUSE_FROM MRPch)
ENDIF()
