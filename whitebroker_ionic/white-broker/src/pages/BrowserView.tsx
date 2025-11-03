import { IonPage, IonContent } from '@ionic/react';
import { useEffect } from 'react';
import { Browser } from '@capacitor/browser';
import './BrowserView.css';

// URL сайта, который нужно открыть
const DEFAULT_URL = 'https://wb.easy-prog.ru/login';

const BrowserView: React.FC = () => {
  useEffect(() => {
    // Открываем URL во внешнем браузере устройства
    const openBrowser = async () => {
      await Browser.open({ 
        url: DEFAULT_URL,
        // windowName: '_system' открывает во внешнем браузере
        windowName: '_system'
      });
    };
    
    openBrowser();
  }, []);

  return (
    <IonPage>
      <IonContent fullscreen className="browser-content">
        <div style={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          height: '100%',
          padding: '20px',
          textAlign: 'center'
        }}>
          <h2>White Broker</h2>
          <p>Сайт открывается в браузере...</p>
          <p style={{ fontSize: '14px', color: '#666', marginTop: '20px' }}>
            {DEFAULT_URL}
          </p>
          <button 
            onClick={async () => {
              await Browser.open({ url: DEFAULT_URL, windowName: '_system' });
            }}
            style={{
              marginTop: '30px',
              padding: '12px 24px',
              backgroundColor: '#512BD4',
              color: 'white',
              border: 'none',
              borderRadius: '8px',
              fontSize: '16px',
              cursor: 'pointer'
            }}
          >
            Открыть сайт
          </button>
        </div>
      </IonContent>
    </IonPage>
  );
};

export default BrowserView;

