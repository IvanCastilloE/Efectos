using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Reproductor
{
    class EfectoDelay : ISampleProvider
    {
        private ISampleProvider fuente;

        public int offsetMiliSegundos;

        private List<float> muestras = new List<float>();
        private int tamañoBuffer;

        private int cantidadMuestrasTranscurridas = 0;
        private int cantidadMuestrasBorradas = 0;

        public EfectoDelay(ISampleProvider fuente, int offsetMiliSegundos)
        {
            this.fuente = fuente;
            this.offsetMiliSegundos = offsetMiliSegundos;

            tamañoBuffer = fuente.WaveFormat.SampleRate * 20 * fuente.WaveFormat.Channels;
        }

        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);
            //Cálculo de tiempo
            float milisegundosTranscurridos = ((float)(cantidadMuestrasTranscurridas) / 
                (float)(fuente.WaveFormat.SampleRate*fuente.WaveFormat.Channels))*1000.0f;
            int numeroMuestrasOffset = (int)((offsetMiliSegundos / 1000.0f)* (float)fuente.WaveFormat.SampleRate);

            //Llenar el buffer
            for (int i=0; i <read; i++)
            {
                muestras.Add(buffer[i + offset]);
            }
            //Si el buffer excede el tamaño, ajustar el numero de elementos
            if(muestras.Count> tamañoBuffer)
            {
                //Eliminar el excedente
                int diferencia = muestras.Count - tamañoBuffer;
                muestras.RemoveRange(0, diferencia);
                cantidadMuestrasBorradas += diferencia;
            }
            //Aplicar el efecto
            if(milisegundosTranscurridos > offsetMiliSegundos)
            {
                for (int i=0; i<read; i++)
                {
                    buffer[i + offset] += muestras[(cantidadMuestrasTranscurridas - cantidadMuestrasBorradas) + i - numeroMuestrasOffset];
                }
            }

            cantidadMuestrasTranscurridas += read;


            return read;
        }
    }
}
