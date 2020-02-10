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

            //Aplicar el efecto
            muestrasLeidas += read;
            segundosTranscuridos = ((float)muestrasLeidas / (float)fuente.WaveFormat.SampleRate) / (float)fuente.WaveFormat.Channels;
            if (segundosTranscuridos >= inicio)
            {
               if (segundosTranscuridos <= duracion)
                        {
                            //Aplicar el efecto
                            float factorEscala = duracion/ segundosTranscuridos  ;
                            for (int i = 0; i < read; i++)
                            {
                                buffer[i + offset] *= factorEscala;
                            }
                        }
            }
         

            return read;
        }
    }
}
