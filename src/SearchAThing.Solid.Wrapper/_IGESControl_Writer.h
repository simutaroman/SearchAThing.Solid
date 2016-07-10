/*
* Thirdy Part Components
* ======================
* SearchAThing.Solid project uses libraries from the following projects:
*
* - OpenCascade [LGPL-2.1](LICENSE.Thirdy/OpenCascade)
* - FreeImage [FreeImage Public License - Version 1.0](LICENSE.Thirdy/FreeImage/license-fi.txt)
* - FreeType [The FreeType Project LICENSE](LICENSE.Thirdy/Freetype/FTL.TXT)
* - GL2PS [GL2PS LICENSE - Version 2, November 2003](LICENSE.Thirdy/gl2ps/COPYING.GL2PS.txt)
* - Qt486 [LGPL 2.1](LICENSE.Thirdy/Qt486/LICENSE.LGPL.txt)
* - tbb [GPL](LICENSE.Thirdy/tbb/COPYING.txt) with [exceptions](https://www.threadingbuildingblocks.org/licensing)
* - tcltk [ActiveTcl Community License Agreement](LICENSE.Thirdy/tcltk/license-at8.6-thread.terms.txt)
* - vtk [Copyright (c) 1993-2008 Ken Martin, Will Schroeder, Bill Lorensen](LICENSE.Thirdy/vtk/Copyright.txt)
*
* SearchAThing.Solid
* ==================
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/

#pragma once

using namespace msclr::interop;
using namespace System;
using namespace System::Diagnostics;

#include <gp_Pnt.hxx>

#include <IGESControl_Controller.hxx>
#include <IGESControl_Writer.hxx>
#include <BRepBuilderAPI_MakeEdge.hxx>
#include <BRepBuilderAPI_MakeWire.hxx>
#include <BRepBuilderAPI_MakeFace.hxx>
#include <TopoDS_Edge.hxx>
#include <TopoDS_Wire.hxx>
#include <TopoDS_Shape.hxx>
#include <TopoDS_Face.hxx>

#include "_TopoDS_Shape.h"

namespace SearchAThing::Solid::Wrapper {

	public ref class _IGESControl_Writer
	{

	public:
		_IGESControl_Writer(String ^ _unit, const Standard_Integer modecr)
		{
			string unit;
			MarshalString(_unit, unit);

			m_Impl = new IGESControl_Writer(unit.c_str(), modecr);
		}

		~_IGESControl_Writer()
		{
			delete m_Impl;
		}

		Standard_Boolean AddShape(_TopoDS_Shape ^sh)
		{
			return m_Impl->AddShape(*(sh->GetTopoDS_Shape()));
		}

		Standard_Boolean AddGeom(const Handle(Standard_Transient)& geom)
		{
			return m_Impl->AddGeom(geom);
		}

		Standard_Boolean AddEntity(const Handle(IGESData_IGESEntity)& ent)
		{
			return m_Impl->AddEntity(ent);
		}

		void ComputeModel()
		{
			m_Impl->ComputeModel();
		}

		Standard_Boolean Write(Standard_OStream& S, const Standard_Boolean fnes)
		{
			return m_Impl->Write(S, fnes);
		}

		Standard_Boolean Write(String ^ _file, const Standard_Boolean fnes)
		{
			string file;
			MarshalString(_file, file);

			return m_Impl->Write(file.c_str(), fnes);
		}

	protected:
		!_IGESControl_Writer()
		{
			delete m_Impl;
		}

	private:
		IGESControl_Writer *m_Impl;

	};

}
