﻿using System;
using System.Collections.Generic;

namespace Scrips {
	public class MyDebugging {
		public static String ListToString<T>(List<T> list) {
			String s = "";

			foreach (T t in list) {
				s += t.ToString() + " ";
			}

			return s;
		}
	}
}
