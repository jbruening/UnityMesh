UnityMesh
=========

based heavily on this: http://forum.unity3d.com/threads/192077-Community-Format-Proposal-UNITYMESH?p=1309128#post1309128

The format is as described later in the thread, with invididual data being separated into chunks, with each chunk having an identifier.

chunks are identified as follows, currently
- 1: vertices
- 2: uvs
- 3: triangles (not used by default)
- 4: submeshes (used instead of triangles by default)
- 5: normals
- 6: tangents
- 7: vertex colors
- 8: bone weights
- 9: bind poses

Build Instructions
====
Open sln in visual studio, press f6. Should output to a bin folder in the root

License: 
---
UnityMesh is distributed under the MIT license as following:

The MIT License (MIT) Copyright (c) 2013 Justin Bruening jubruening@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
