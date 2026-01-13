Kurulum: Öncelikle 
1 - appsettings.Development 'da ilgili mssql connection bilgilerini giriyoruz. bir tanesi MasterConnection(Database create etmek için) diğer connection ise(AppConnection) tüm işlemleri yapacağımız connection'ımız.
2 - Proje ayağa kalkarken tüm table,stored procedureleri ve örnek kayıtları insert etmektedir.
2- Connection bilgilerini girildiyse direk projeyi çalıştırıp Swagger üzerinden Kullanıcı adı: admin ve password=123456 yazıp token alarak tüm işlemleri gerçekleştirebilirsiniz.

Kısa Açıklama:
Proje Clean Architecture yaklaşımıyla yapılmıştır. Katmanlar aşağıdaki gibi organize edilmiştir:
Context: Dapper ile veri erişimi sağlayan context sınıfı.
Controllers: API endpointlerini expose eden katman.
Database: Database başlangıç ve init işlemlerini yöneten servisler.
Dtos: API üzerinden veri alışverişi için kullanılan veri transfer objeleri.
Entity: Veritabanı tablolarına karşılık gelen modeller.
Helpers: Şifrelemeye yardımcı fonksiyonlar.
Repository: Veritabanı işlemlerini yapan repository katmanı.
Service: İş mantığını yönetilen alan.

Authentication & Token Özet:
Uygulamada kullanıcı girişleri AuthService ve IUsersRepository üzerinden doğrulanır. Kullanıcı adı ve şifre doğruysa, AuthController tarafından JWT(JSON Web Token) üretilir ve client’a gönderilir. Token, kullanıcı adı ve rol bilgilerini içerir ve belirli bir süre (örneğn 30 dakika) geçerli olur. Client, sonraki API isteklerinde bu token’ı Authorization header’da göndererek yetkilendirilmiş erişim sağlar.
