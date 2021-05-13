use libc::c_char;
use std::ffi::CStr;
use std::ffi::CString;
use std::slice;

#[no_mangle]
pub extern "C" fn add_numbers(number1: i32, number2: i32) -> i32 {
    number1 + number2
}

#[no_mangle]
pub extern "C" fn string_from_rust(buffer_ptr: *mut c_char)  {
    let ss : &str = "Howdy";

    let s = CString::new(ss).unwrap();
    unsafe {
        libc::strcpy(buffer_ptr, s.as_ptr());
    }
}

#[no_mangle]
pub extern "C" fn string_from_rust_intptr(buffer_ptr: *mut c_char)  {
    let ss : &str = "Howdy";

    let s = CString::new(ss).unwrap();
    unsafe {
        libc::strcpy(buffer_ptr, s.as_ptr());
    }
}

#[no_mangle]
pub extern "C" fn string_to_rust(ptr: *const c_char) {
    let cstr = unsafe { CStr::from_ptr(ptr) };

    match cstr.to_str() {
        Ok(s) => {
            // Here `s` is regular `&str` and we can work with it
            println!("Howdy {} from Rust", s);
        }
        Err(_) => {
            // handle the error
        }
    }
}

