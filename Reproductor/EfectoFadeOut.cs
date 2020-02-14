using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace Reproductor
{
    class EfectoFadeOUt :ISampleProvider
    {
        //entrada
        private ISampleProvider fuente;
        private int muestrasLeidas = 0;
        private float segundosTranscuridos = 0;
        private float duracion;
        private float inicio;
        public EfectoFadeOUt(ISampleProvider fuente, float duracion, float inicio)
        {
            this.fuente = fuente;
            this.duracion = duracion;
            this.inicio = inicio;
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
            //Proceso - modificacion de los valores del buffer
            //Aplicar el efecto
            muestrasLeidas += read;
            segundosTranscuridos = ((float)muestrasLeidas / (float)fuente.WaveFormat.SampleRate) 
                / (float)fuente.WaveFormat.Channels;
            //Aplicar el efecto
            if(segundosTranscuridos >= inicio && segundosTranscuridos <= duracion)
            {
                float factorEscala = (((inicio+duracion)-segundosTranscuridos)/duracion);
                if((inicio + duracion) - segundosTranscuridos <= 0.0f)
                {
                    factorEscala = 0.0f;
                }
                //Escalamos muestra
                for (int i = 0; i < read; i++)
                {
                    buffer[i + offset] *= factorEscala;
                }
                //Salida -la variable buffer modificada
            }
            return read;
        }
    }
}
